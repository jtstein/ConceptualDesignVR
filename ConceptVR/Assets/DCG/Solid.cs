using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.Linq;

public class Solid : DCGElement {
    public List<Face> faces;
    Mesh mesh;

    public Solid()
    {
        /*faces = new List<Face>();
        DCGBase.solids.Add(this);*/
    }

    public Solid(List<Face> faces)
    {
        this.faces = faces;
        foreach (Face f in faces)
            f.solids.Add(this);

        elementID = nextElementID();
        DCGBase.solids.Add(this);
        DCGBase.all.Add(elementID, this as DCGElement);
        
        if (NetPlayer.local != null)
        {
            int[] pointIDs = new int[faces.Count];
            for (int i = 0; i < faces.Count; ++i)
                pointIDs[i] = faces[i].elementID;
            DCGBase.synch.CmdAddElement(elementID, pointIDs, ElementType.solid, NetPlayer.local.playerID);
        }
    }

    //Network constructor
    public Solid(List<Face> faces, int netID)
    {
        this.faces = faces;
        foreach (Face f in faces)
            f.solids.Add(this);

        elementID = netID;
        DCGBase.solids.Add(this);
        DCGBase.all.Add(elementID, this as DCGElement);
    }

    public Solid(Mesh m, Matrix4x4 t, Vector3 translate)
    {
        mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        ArrayList points = new ArrayList();
        List<int> pointMap = new List<int>();
        faces = new List<Face>();

        foreach (Vector3 v in m.vertices)
        {
            int match = -1;
            Vector3 tv = (Vector3)(t * v) + translate;
            for (int i = 0; i < points.Count; ++i)
            {
                Point p = (Point)points[i];
                if (Vector3.Distance(p.position, tv) <= .0001f)
                {
                    match = i;
                    break;
                }
            }
            if (match == -1)
            {
                verts.Add(tv);
                points.Add(new Point(tv));
                pointMap.Add(points.Count-1);
            } else
            {
                pointMap.Add(match);
            }
        }

        for (int tri = 0; tri < m.triangles.Length - 2; tri += 3)
        {
            tris.Add(pointMap[m.triangles[tri]]);
            tris.Add(pointMap[m.triangles[tri+1]]);
            tris.Add(pointMap[m.triangles[tri+2]]);

            Point p1 = (Point)points[pointMap[m.triangles[tri]]];
            Point p2 = (Point)points[pointMap[m.triangles[tri+1]]];
            Point p3 = (Point)points[pointMap[m.triangles[tri+2]]];

            List<Edge> edges = new List<Edge>();
            edges.Add(new Edge(p1, p2));
            edges.Add(new Edge(p2, p3));
            edges.Add(new Edge(p3, p1));

            Face f = new Face(edges);
            faces.Add(f);
            f.solids.Add(this);
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris.ToArray(), 0);


        elementID = nextElementID();
        DCGBase.solids.Add(this);
        DCGBase.all.Add(elementID, this as DCGElement);


        if (NetPlayer.local != null)
        {
            int[] pointIDs = new int[faces.Count];
            for (int i = 0; i < faces.Count; ++i)
                pointIDs[i] = faces[i].elementID;
            DCGBase.synch.CmdAddElement(elementID, pointIDs, ElementType.solid, NetPlayer.local.playerID);
        }
    }

    public Mesh getMesh()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        int offset = 0;
        foreach (Face f in faces)
        {
            verts.AddRange(f.mesh.vertices);
            foreach (int i in f.mesh.triangles)
                tris.Add(i + offset);
            offset += f.mesh.vertexCount;
        }

        Mesh m = new Mesh();
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);

        return m;
    }

    public override void Render(Material mat = null)
    {
        foreach (Face f in faces)
            f.Render(mat);
    }

    public override void Update()
    {
        //TODO
    }

    public override void Remove()
    {
        foreach (Face f in faces)
            f.solids.Remove(this);
        DCGBase.solids.Remove(this);
        DCGBase.synch.CmdRemoveElement(elementID, NetPlayer.local.playerID);
    }
    public override void RemoveChildren()
    {
        foreach (Face f in faces)
            if (DCGBase.sElements.Contains(f))
                DCGBase.sElements.Remove(f);
    }

    public override bool ChildrenSelected()
    {
        foreach (Face e in faces)
            if (!e.isSelected && !e.ChildrenSelected())
                return false;
        return true;
    }

    public List<Point> getPoints()
    {
        List<Point> points = new List<Point>();
        foreach(Face f in faces)
        {
            points.AddRange(f.GetPoints());
        }
        return points.Distinct().ToList();
    }
    public List<Edge> getEdges()
    {
        List<Edge> edges = new List<Edge>();
        foreach(Face f in faces)
        {
            edges.AddRange(f.edges);
        }
        return edges.Distinct().ToList();
    }
    public override void Lock()
    {
        foreach (Face f in faces)
        {
            if (!f.isLocked)
                f.Lock();
        }
        isLocked = true;
    }
    public override void Unlock()
    {
        foreach (Face f in faces)
        {
            if (f.isLocked)
                f.Unlock();
        }
        isLocked = false;
    }

    //2D cross product of the xy components to vectors a and b
    private float zCross(Vector3 a, Vector3 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    public bool ContainsPoint(Vector3 p)
    {
        int colCount = 0;
        foreach(Face f in faces)
        {
            for (int i = 0; i < f.subTriangles.Count; i += 3)
            {
                Vector3 triI = f.subTriangles[i+1] - f.subTriangles[i];
                Vector3 triJ = f.subTriangles[i+2] - f.subTriangles[i];
                Vector3 pDiff = p - f.subTriangles[i];

                Vector3 pProj = f.subTriangles[i] - zCross(pDiff, triI)*triI + zCross(pDiff, triJ)*triJ;

                float signA = Mathf.Sign(zCross(f.subTriangles[i + 1] - f.subTriangles[i], pDiff));
                float signB = Mathf.Sign(zCross(f.subTriangles[i + 2] - f.subTriangles[i + 1], pDiff));
                float signC = Mathf.Sign(zCross(f.subTriangles[i] - f.subTriangles[i + 2], pDiff));

                if (signA == signB && signB == signC && pProj.z == p.z)
                {
                    colCount++;
                }
            }
        }
        return colCount % 2 == 1;
    }

    public static Solid FindClosedSurface(Point start)
    {
        List<Point> connected = start.GetConnectedPoints(-1);   //Get the set of points we can path to

        foreach (Point p in connected)  //Check for holes in the surface encompassed by the set of points
        {
            if (p.AdjacentFaces().Count != p.edges.Count)
                return null;
        }

        List<Face> faces = start.edges[0].faces[0].GetConnectedFaces(-1);
        //Debug.Log("Faces: " + faces.Count);
        return new Solid(faces);
    }
    public override List<DCGElement> GetChildren()
    {
        List<DCGElement> elems = new List<DCGElement>();
        foreach (Face f in faces){
            elems.Add(f);
            elems.AddRange(f.GetChildren());
        }
        return elems.Distinct().ToList();
    }
    public void addPointsToDCG()
    {
        List<Point> points = getPoints();
        foreach (Point p in points)
            if (!DCGBase.sPoints.Contains(p))
                DCGBase.sPoints.Add(p);
    }
    public override DCGElement Copy(int moveId = -1)
    {
        Solid copy = (Solid)lastCopyMade;
        if (this.lastMoveID != moveId)
        {
            List<Face> cFaces = new List<Face>();
            foreach (Face f in faces)
            {
                Face edgeCopy = (Face)f.Copy(moveId);
                if (edgeCopy != null && !cFaces.Contains(edgeCopy))
                {
                    cFaces.Add(edgeCopy);
                }
            }
            cFaces = cFaces.Distinct().ToList();
            copy = new Solid(cFaces);
        }
        lastCopyMade = copy;
        return copy;
    }
}
