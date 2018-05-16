using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonTool : Tool {
    public GameObject linePrefab;
    //public DebugGrapher graph;

    LineRenderer currentLine;
    List<Vector3> currentPositions;
    GameObject generatedObj;
    const float circleSnap = 0.00f;
    DCGBase dcgBase;

    // Use this for initialization
    void Start()
    {
        dcgBase = GameObject.Find("DCG").GetComponent<DCGBase>();
        //graph = GameObject.Find("PolygonDebugGrapher").GetComponent<DebugGrapher>();
    }

    // Update is called once per frame
    new void Update()
    {
        if (triggerInput && !currentLine.loop)
        {
            currentPositions.Add(controllerPosition);
            currentLine.positionCount = currentPositions.Count;
            currentLine.SetPositions(currentPositions.ToArray());
        }
    }

    public override bool TriggerDown()
    {
        currentPositions = new List<Vector3>();
        currentLine = Instantiate(linePrefab).GetComponent<LineRenderer>();
        return true;
    }

    public override bool TriggerUp()
    {
        SimplifyCurrent();
        GlomCurrent(.005f);

        BuildCurrent();

        Destroy(currentLine);
        /*currentLine.positionCount = currentPositions.Count;
        currentLine.SetPositions(currentPositions.ToArray());
        currentLine.loop = true;*/
        return true;
    }

    void BuildCurrent()
    {
        List<Vector3> normals = Enumerable.Repeat(Vector3.up, currentPositions.Count()).ToList();
        List<Point> points = vec3toPoints(currentPositions);
        List<Edge> edges = pointsToEdges(points);
        new Face(edges);
    }

    void GlomCurrent(float limit)
    {
        List<Vector3> newPositions = new List<Vector3>();
        newPositions.Add(currentPositions[0]);
        Vector3 prev = currentPositions[0];
        foreach (Vector3 v in currentPositions)
        {
            if ((v - prev).sqrMagnitude < limit * limit)
            {
                newPositions[newPositions.Count - 1] = ((v + prev) / 2);
            }
            else
            {
                newPositions.Add(v);
                prev = v;
            }
        }

        currentPositions = newPositions;
    }

    void SmoothCurrent()
    {
        List<Vector3> newPositions = new List<Vector3>(currentPositions.Count);
        newPositions.Add(currentPositions[0]);
        for (int i = 1; i < currentPositions.Count - 1; ++i)
        {
            newPositions.Add((currentPositions[i - 1] + currentPositions[i + 1]) / 2);
        }
        newPositions.Add(currentPositions[currentPositions.Count - 1]);
        currentPositions = newPositions;
    }


    const float pLimit = .002f;    //Minimum sharpness prominence to be included as a vertex --Lewi-- Updated this to .5f. We were getting a ton of verticies.
    void SimplifyCurrent()
    {
        //Debug.Log(currentPositions.Count.ToString());
        Vector3[] pos = currentPositions.ToArray();
        int count = currentPositions.Count;
        float[] sharpness = new float[count];
        int min = 0;
        float speed = 0;

        //graph.length = count;   //Debug

        for (int i = 0; i < count; ++i)
            speed += Vector3.Distance(pos[(i + count - 1) % count], pos[i]);
        speed /= count;

        for (int i = 0; i < count; ++i)
        {
            Vector3 p = pos[(i + count - 1) % count];
            Vector3 n = pos[(i + 1) % count];
            Vector3 v = pos[i];
            sharpness[i] = Vector3.AngleBetween(v - p, n - v) / (1 + (p - n).sqrMagnitude) * speed;
            if (sharpness[i] < sharpness[min])
                min = i;

            //graph.AddValue("sharpness", sharpness[i], false);   //Debug
        }

        List<TopoPoint> tPoints = new List<TopoPoint>();
        for (int i = 0; i < count; ++i)
        {
            int p = (i + min + count - 1) % count;
            int n = (i + min + 1) % count;
            int v = (i + min) % count;
            if (sharpness[v] < sharpness[p] == sharpness[v] < sharpness[n])
            {
                TopoPoint t = new TopoPoint();
                t.pos = pos[v];
                t.height = sharpness[v];
                t.prominence = 0;
                tPoints.Add(t);
            }
        }

        tPoints[0].parent = tPoints[0];
        TopoPoint[] tArr = tPoints.ToArray();
        TopoTree(tArr, 0, 0);

        currentPositions = new List<Vector3>();
        foreach (TopoPoint t in tArr)
            if (t.prominence >= pLimit)
            {
                currentPositions.Add(t.pos);
            }


    }

    class TopoPoint
    {
        public Vector3 pos;
        public float height;
        public TopoPoint parent;
        public TopoPoint lChild;
        public TopoPoint hChild;
        public float prominence;
    };

    //Builds a tree representing the heirarchy of topographic prominences for points on the line.
    void TopoTree(TopoPoint[] p, int i, float b)
    {
        if (i % 2 == 1)
        {
            //Debug.Log(p[i].height + "  " + p[i].parent.height + "  " + b);
            p[i].prominence = p[i].height - b;
        }
        else
        {
            int maxL = -1;
            int minL = -1;
            int j = (i + p.Length - 1) % p.Length;
            while (p[j].parent == null)
            {
                if (minL == -1 || p[minL].height > p[j].height)
                    minL = j;
                if (maxL == -1 || p[maxL].height < p[j].height)
                    maxL = j;
                j = (j + p.Length - 1) % p.Length;
            }

            int maxR = -1;
            int minR = -1;
            j = (i + 1) % p.Length;
            while (p[j].parent == null)
            {
                if (minR == -1 || p[minR].height > p[j].height)
                    minR = j;
                if (maxR == -1 || p[maxR].height < p[j].height)
                    maxR = j;
                j = (j + 1) % p.Length;
            }

            if (i == 0)
            {
                p[i].hChild = p[minL];
                p[minL].parent = p[i];
                TopoTree(p, minL, b);
            }
            else if (p[maxL].height > p[maxR].height)
            {
                p[i].hChild = p[minL];
                p[i].lChild = p[minR];
                p[minL].parent = p[i];
                p[minR].parent = p[i];
                TopoTree(p, minL, b);
                TopoTree(p, minR, p[i].height);
            }
            else
            {
                p[i].hChild = p[minR];
                p[i].lChild = p[minL];
                p[minR].parent = p[i];
                p[minL].parent = p[i];
                TopoTree(p, minR, b);
                TopoTree(p, minL, p[i].height);
            }
        }
    }

    public List<Point> vec3toPoints(List<Vector3> vectors)
    {
        List<Point> points = new List<Point>();
        foreach (Vector3 v in vectors)
        {
            points.Add(new Point(v));
        }
        return points;
    }
    public List<Edge> pointsToEdges(List<Point> points)
    {
        List<Edge> edges = new List<Edge>();
        Point prev = points[points.Count - 1];
        foreach (Point p in points)
        {
            edges.Add(new Edge(prev, p));
            prev = p;
        }
        //edges.Add(new Edge(points, true));    //BLAME
        return edges;
    }
}
