using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType { point, edge, face, solid, smoothEdge, smoothFace, smoothQuad }

public class DCGBase : MonoBehaviour {
    public Mesh pointMesh;
    public Mesh edgeMesh;
    public Material pointMat;
    public Material edgeMat;
    public Material faceMat;
    public Material solidMat;
    public static List<DCGMaterial> matList = new List<DCGMaterial>();
    public static List<Point> points = new List<Point>();
    public static List<Edge> edges = new List<Edge>();
    public static List<Face> faces = new List<Face>();
    public static List<Solid> solids = new List<Solid>();
    public static List<DCGConstraint> constraints = new List<DCGConstraint>();
    public static List<DCGElement> sElements = new List<DCGElement>();
    public static List<Point> sPoints = new List<Point>();

    public static Dictionary<int, DCGElement> all = new Dictionary<int, DCGElement>();
    public static DCGSynchronizer synch;

    private static int moveID = 0;

    public static DCGBase instance;

	// Use this for initialization
	void Start () {
        instance = this;


        Transform starter = transform.Find("Starter");
        //Mesh starterMesh = starter.gameObject.GetComponent<MeshFilter>().mesh;
        
        //new Solid(starterMesh, Matrix4x4.TRS(starter.position, starter.rotation, starter.localScale), starter.position);

        //starter.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        RenderObject();
	}

    private void RenderObject()
    {
        //Render top-level DCG Elements
        solidMat.SetPass(0);
        foreach (Solid s in solids)
            s.Render();
        foreach (Face f in faces)
            if (f.solids.Count == 0 && !f.isSelected)
                f.Render();


        foreach (DCGMaterial DCGMat in matList) {
            // dip the brush
            
            foreach (Face f in DCGMat.facesUsingMat)
            {
                //if (f.solids.Count == 0 && !f.isSelected)
                    f.Render(DCGMat.mat);
            }
            // clean the brush
            solidMat.SetPass(0);
        }

        foreach (Edge e in edges)
            if (e.faces.Count == 0 && !e.isSelected)
                e.Render();
        foreach (Point p in points)
            if (p.edges.Count == 0 && !p.isSelected)
                p.Render();


        //Render highlights
        pointMat.SetPass(0);
        foreach (Point p in points)
            if (p.edges.Count > 0 && !p.isSelected)
                p.Render(pointMat);

        edgeMat.SetPass(0);
        foreach (Edge e in edges)
            if (e.faces.Count > 0 && !e.isSelected)
                e.Render(edgeMat);
        
        faceMat.SetPass(0);
        foreach (Face f in faces)
            if (f.solids.Count > 0 && !f.isSelected)
                f.Render(faceMat);
    }

    public static int nextMoveID()
    {
        return moveID++;
    }


    public static DCGElement NearestElement(Vector3 pos, float maxDist)
    {
        Point np = NearestPoint(pos, maxDist);
        Edge ne = NearestEdge(pos, maxDist / Mathf.Sqrt(2f));
        Face nf = NearestFace(pos, maxDist / 2f);
        Solid ns = NearestSolid(pos, maxDist / 2f);

        float dp = Mathf.Infinity;
        float de = Mathf.Infinity;
        float df = Mathf.Infinity;

        if (np != null)
            dp = np.Distance(pos);
        if (ne != null)
            de = ne.Distance(pos) * Mathf.Sqrt(2f);
        if (nf != null)
            df = nf.Distance(pos) * 2f;

        float min = Mathf.Min(dp, de, df);

        if (min == dp)
            return np;
        else if (min == de)
            return ne;
        else if (min == df)
            return nf;
        else if (ns != null)
            return ns;
        else
            return null;
    }

    public static Point NearestPoint(Vector3 pos, float maxDist)
    {
        float nDist = maxDist;
        Point nPoint = null;
        foreach (Point p in DCGBase.points)
        {
            float dist = p.Distance(pos);
            if (dist < nDist)
            {
                nDist = dist;
                nPoint = p;
            }
        }

        return nPoint;
    }

    public static Edge NearestEdge(Vector3 pos, float maxDist)
    {
        float nDist = maxDist;
        Edge nEdge = null;
        foreach (Edge e in DCGBase.edges)
        {
            float dist = e.Distance(pos);
            if (dist < nDist)
            {
                nDist = dist;
                nEdge = e;
            }
        }

        return nEdge;
    }

    public static Face NearestFace(Vector3 pos, float maxDist)
    {
        float nDist = maxDist;
        Face nFace = null;
        foreach (Face f in DCGBase.faces)
        {
            float dist = f.Distance(pos);
            if (dist < nDist)
            {
                nDist = dist;
                nFace = f;
            }
        }

        return nFace;
    }

    public static Solid NearestSolid(Vector3 pos, float maxDist)
    {
        foreach (Solid s in solids)
            if (s.ContainsPoint(pos))
                return s;
        return null;
    }

    public void AddConstraint(DCGConstraint con)
    {
        //TODO: Network it
        constraints.Add(con);
    }
    public static void RemoveAll()
    {
        points.Clear();
        edges.Clear();
        faces.Clear();
        all.Clear();
    }
}
