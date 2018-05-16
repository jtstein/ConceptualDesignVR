using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//Deprecated
public class SketchTool : Tool {
    public GameObject linePrefab;
    
    LineRenderer currentLine;
    List<Vector3> currentPositions;
    GameObject generatedObj;
    const float circleSnap = 0.00f;
    DCGBase dcgBase; 

	// Use this for initialization
	void Start () {
        dcgBase = GameObject.Find("DCG").GetComponent<DCGBase>();
    }
	
	// Update is called once per frame
	void Update () {
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

        currentLine.positionCount = currentPositions.Count;
        currentLine.SetPositions(currentPositions.ToArray());
        currentLine.loop = true;
        //Generate the circle
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
        foreach(Vector3 v in currentPositions)
        {
            if ((v-prev).sqrMagnitude < limit*limit)
            {
                newPositions[newPositions.Count - 1] = ((v + prev) / 2);
            } else
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

        for (int i = 0; i < count; ++i)
            speed += Vector3.Distance(pos[(i + count - 1) % count], pos[i]);
        speed /= count;

        for (int i = 0; i < count; ++i)
        {
            Vector3 p = pos[(i + count - 1) % count];
            Vector3 n = pos[(i + 1) % count];
            Vector3 v = pos[i];
            sharpness[i] = Vector3.AngleBetween(v - p, n - v) / (1 + (p-n).sqrMagnitude) * speed;
            if (sharpness[i] < sharpness[min])
                min = i;
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
        foreach(TopoPoint t in tArr)
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
        foreach(Vector3 v in vectors)
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






























    Vector3[] addMeshNormalstoVerts(Vector3[] verts, Vector3 norms)
    {
        for (int i = 0; i < verts.Length; ++i)
        {
            verts[i] += norms/4;
        }
        return verts;
    }


    List<int> addTrianglesForCircle(List<int> triangles)
    {
        int last = 59;
        for (int i = 0; i < 60; ++i)
        {
            //Need to flip the ored to ensure properfacing triangles
            if (i % 2 == 0)
            {
                triangles.Add(60);
                triangles.Add(i);
                triangles.Add(last);
            }
            else
            {
                triangles.Add(60);
                triangles.Add(last);
                triangles.Add(i);
            }
            if (i == 59)
            {
                triangles.Add(60);
                triangles.Add(i);
                triangles.Add(0);
            }
            last = i;
        }
        return triangles;
    }
    List<int> addTrianglesForCylinder(List<int> triangles)
    {
        int j = 60;
        for (int i = 0; i < 60; ++i)
        {
            //Need to flip the ored to ensure properfacing triangles
            if (i % 2 == 0)
            {
                triangles.Add(i);
                triangles.Add(j);
                triangles.Add(i+1);

            }
            else if (i == 59)
            {
                triangles.Add(i);
                triangles.Add(60);
                triangles.Add(0);
            }
            else
            {
                triangles.Add(i+1);
                triangles.Add(j);
                triangles.Add(i);

            }
            ++j;
        }
        j = 0;
        for (int i = 60; i < 120; ++i)
        {
            //Need to flip the ored to ensure properfacing triangles
            if (i % 2 == 0)
            {
                triangles.Add(i + 1);
                triangles.Add(j + 1);
                triangles.Add(i);

            }
            else if (i == 119)
            {
                triangles.Add(60);
                triangles.Add(0);
                triangles.Add(i);
            }
            else
            {
                triangles.Add(i + 1);
                triangles.Add(j+1);
                triangles.Add(i);
                triangles.Add(i);
                triangles.Add(j + 1);
                triangles.Add(i + 1);

            }
            ++j;
        }
        Debug.Log(Mathf.Max(triangles.ToArray()));
        return triangles;
    }

    void makeCircle()
    {
        if (Vector3.Distance(currentPositions[0], currentPositions[currentPositions.Count - 1]) < circleSnap)
        {
            Vector3 avgPos = new Vector3(0f, 0f, 0f);
            Vector3 avgNormal = new Vector3(0f, 0f, 0f);
            Vector3 prevSegment = currentPositions[currentPositions.Count - 1] - currentPositions[0];
            Vector3 prevPos = currentPositions[currentPositions.Count - 1];
            for (int i = 0; i < currentPositions.Count; ++i)
            {
                Vector3 v = currentPositions[i];
                avgPos += v;
                Vector3 segment = v - prevPos;

                avgNormal += Vector3.Cross(prevSegment.normalized, segment.normalized).normalized;
                //Debug.Log(Vector3.Cross(prevSegment.normalized, segment.normalized).normalized);

                prevSegment = segment;
                prevPos = v;
            }
            avgPos /= currentPositions.Count;
            avgNormal.Normalize();

            float avgRadius = 0f;
            foreach (Vector3 v in currentPositions)
                avgRadius += Vector3.Distance(v, avgPos);
            avgRadius /= currentPositions.Count;

            Vector3[] circlePoints = new Vector3[60];
            for (int i = 0; i < 60; ++i)
            {
                circlePoints[i] = avgPos + Quaternion.AngleAxis(i * 6f, avgNormal) * (Vector3.Cross(Vector3.up, avgNormal) * avgRadius);
            }

            currentLine.positionCount = 60;
            currentLine.loop = true;
            currentLine.SetPositions(circlePoints);
            currentLine.GetComponent<Renderer>().enabled = false;

            //Get verticies of generated circle
            List<Vector3> verts = new List<Vector3>(circlePoints);
            verts.Add(avgPos);
            List<Vector3> normals = Enumerable.Repeat(Vector3.up, 61).ToList();

            //Generate triangles of the face
            List<int> triangles = new List<int>();
            triangles = addTrianglesForCircle(triangles);

            //Generate the face of the circle that we just drew
            GameObject circle = new GameObject();
            circle.AddComponent<MeshFilter>();
            circle.AddComponent<MeshRenderer>();
            circle.GetComponent<MeshFilter>().mesh = new Mesh();
            Mesh mesh = circle.GetComponent<MeshFilter>().mesh;
            mesh.vertices = verts.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
            circle.GetComponent<MeshRenderer>().material = currentLine.material;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            //currently attempting to make a duplicate circle
            GameObject secondCircle = new GameObject();
            secondCircle.AddComponent<MeshFilter>();
            secondCircle.AddComponent<MeshRenderer>();
            secondCircle.GetComponent<MeshRenderer>().material = circle.GetComponent<MeshRenderer>().material;
            Mesh secondMesh = secondCircle.GetComponent<MeshFilter>().mesh;
            secondMesh.vertices = addMeshNormalstoVerts(mesh.vertices, mesh.normals[60]);
            secondMesh.normals = mesh.normals;
            secondMesh.triangles = addTrianglesForCircle(new List<int>()).ToArray();
            secondMesh.RecalculateNormals();
            secondMesh.RecalculateBounds();
            //^hey that works

            //now to generate the cylinder
            GameObject cyl = new GameObject();
            cyl.AddComponent<MeshFilter>();
            cyl.AddComponent<MeshRenderer>();
            cyl.GetComponent<MeshRenderer>().material = circle.GetComponent<MeshRenderer>().material;
            cyl.GetComponent<MeshFilter>().mesh = new Mesh();
            Mesh cylMesh = cyl.GetComponent<MeshFilter>().mesh;
            List<Vector3> sidesVerts = new List<Vector3>(mesh.vertices);
            sidesVerts.AddRange(secondMesh.vertices.ToList<Vector3>());
            //Get rid of the center point verticies of the faces
            sidesVerts.RemoveAt(60);
            sidesVerts.RemoveAt(120);
            Debug.Log(sidesVerts.Count);
            List<Vector3> sidesNormals = Enumerable.Repeat(Vector3.up, 120).ToList();
            Debug.Log("Cyl Verts Size: " + sidesVerts.Count.ToString());
            Debug.Log("Cyl Norm Size: " + sidesNormals.Count.ToString());
            cylMesh.vertices = sidesVerts.ToArray();
            cylMesh.normals = sidesNormals.ToArray();
            cylMesh.triangles = addTrianglesForCylinder(new List<int>()).ToArray();
            cylMesh.RecalculateNormals();
            cylMesh.RecalculateBounds();
            MeshFilter[] filters = { circle.GetComponent<MeshFilter>(), secondCircle.GetComponent<MeshFilter>(), cyl.GetComponent<MeshFilter>() };
            CombineInstance[] combine = new CombineInstance[filters.Length];
            for (int i = 0; i < filters.Length; ++i)
            {
                combine[i].mesh = filters[i].sharedMesh;
                combine[i].transform = filters[i].transform.localToWorldMatrix;
                filters[i].gameObject.SetActive(false);
            }
            generatedObj = new GameObject();
            generatedObj.AddComponent<MeshFilter>();
            generatedObj.AddComponent<MeshRenderer>();
            generatedObj.GetComponent<MeshRenderer>().material = circle.GetComponent<MeshRenderer>().material;
            generatedObj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            generatedObj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            generatedObj.SetActive(true);



        }
        else
        {
            currentLine.positionCount = currentPositions.Count;
            currentLine.SetPositions(currentPositions.ToArray());
        }
    }
}
