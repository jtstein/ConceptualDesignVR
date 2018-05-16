using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Face : DCGElement
{
    public List<Edge> edges;   //The edges of this face
    public List<Solid> solids; //Any solids that this face is a part of.  Since we're in 3D, this should logically only ever be 1.
    public List<Vector3> subTriangles;  //The triangles this face is made up of
    public Mesh mesh;
    public DCGMaterial mat;
    public bool isAwful;
    bool normalConfident;

    public Face()
    {
        /*solids = new List<Solid>();
        edges = new List<Edge>();
        elementID = nextElementID();

        DCGBase.faces.Add(this);*/
    }

    public Face(List<Edge> edges)
    {
        solids = new List<Solid>();
        this.edges = edges;
        foreach (Edge e in edges)
            e.faces.Add(this);

        elementID = nextElementID();
        DCGBase.faces.Add(this);
        DCGBase.all.Add(elementID, this as DCGElement);

        updateAwful();
        updateMesh();

        if (NetPlayer.local != null)
        {
            int[] pointIDs = new int[edges.Count];
            for (int i = 0; i < edges.Count; ++i)
                pointIDs[i] = edges[i].elementID;
            DCGBase.synch.CmdAddElement(elementID, pointIDs, ElementType.face, NetPlayer.local.playerID);
        }
    }

    //Network constructor
    public Face(List<Edge> edges, int netID)
    {
        solids = new List<Solid>();
        this.edges = edges;
        foreach (Edge e in edges)
            e.faces.Add(this);

        elementID = netID;
        DCGBase.faces.Add(this);
        DCGBase.all.Add(elementID, this as DCGElement);

        updateAwful();
        updateMesh();
    }

    public override void Render(Material mat = null)
    {
        if (mat == null)
            mat = DCGBase.instance.solidMat;
        //Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity,0);
        Graphics.DrawMesh(mesh, Matrix4x4.identity, mat, 0);
    }

    public override void Update()
    {
        updateAwful();
        updateMesh();
    }

    public override void Remove()
    {
        for (int i = solids.Count-1; i >= 0; --i)
            solids[i].Remove();
        foreach (Edge e in edges)
            e.faces.Remove(this);
        if(mat != null)
            mat.RemoveFace(this);
        DCGBase.faces.Remove(this);
        DCGBase.synch.CmdRemoveElement(elementID, NetPlayer.local.playerID);
    }
    public override void RemoveChildren()
    {
        foreach (Edge e in edges)
            if (DCGBase.sElements.Contains(e))
                DCGBase.sElements.Remove(e);
    }

    public override bool ChildrenSelected()
    {
        foreach (Edge e in edges)
            if (!e.isSelected && !e.ChildrenSelected())
                return false;
        return true;
    }

    public override float Distance(Vector3 pos)
    {
        List<Point> fp = GetPoints();   //face points
        List<Vector2> pp = new List<Vector2>();     //projected points
        Vector3 zvec = getNormal();
        Vector3 yvec = fp[1].position - fp[0].position;
        Vector3 grabProj3 = Vector3.ProjectOnPlane(pos, zvec);
        Vector3 grabProjY = Vector3.Project(grabProj3, yvec);
        Vector2 grab = new Vector2((grabProj3 - grabProjY).magnitude, grabProjY.magnitude);

        Vector3 posRel = pos - fp[0].position;
        float dist2 = (posRel - Vector3.ProjectOnPlane(posRel, zvec)).sqrMagnitude;

        foreach (Point p in fp)
        {
            Vector3 proj3 = Vector3.ProjectOnPlane(p.position, zvec);
            Vector3 projY = Vector3.Project(proj3, yvec);
            pp.Add(new Vector2((proj3 - projY).magnitude, projY.magnitude));
        }


        //Check if the projected poly contains the projected controller
        float count = 0;
        Vector2 prev = pp[pp.Count - 1];
        foreach (Vector2 cur in pp)
        {
            if ((prev.y > grab.y != cur.y > grab.y) && (grab.x < Mathf.Lerp(prev.x, cur.x, (grab.y - prev.y) / (cur.y - prev.y))))
                count++;
            prev = cur;
        }


        if (count % 2 == 1)
            return Mathf.Sqrt(dist2);
        else return Mathf.Infinity;
    }

    //Constraints are index of subtriangle, and proportions of axes (b-a) and (c-a)
    public override DCGConstraint NearestConstraint(Vector3 pos)
    {
        float nz = Mathf.Infinity;
        float ni = 0;
        float nx = 0;
        float ny = 0;
        Vector3 nPos = Vector3.zero;

        for (int i = 0; i < subTriangles.Count; i += 3)
        {
            Vector3 a = subTriangles[i];        //Fetch vertices
            Vector3 b = subTriangles[i + 1];
            Vector3 c = subTriangles[i + 2];

            Vector3 diff = pos - a;     //position of pos relative to this triangle
            Vector3 tx = (b - a);       //X vector of this triangle's subspace
            Vector3 ty = (c - a);       //Y vector of this triangle's subspace
            Vector3 tz = Vector3.Cross(tx, ty).normalized;  //Normal of this triangle

            float Z = Vector3.Dot(diff, tz);
            float pX = Vector3.Dot(diff, tx) / tx.sqrMagnitude;
            float pY = Vector3.Dot(diff, ty) / ty.sqrMagnitude;
            if (Z < nz && pX > 0 && pY > 0 && pX+pY <= 1)
            {
                nz = Z;
                nPos = a + tx * pX + ty * pY;
                nx = pX;
                ny = pY;
                ni = i;
            }
        }
        
        DCGConstraint con = new DCGConstraint();
        con.constrainerID = elementID;
        con.constraintData = new float[] {ni, nx, ny};

        return con;
    }

    public override Vector3 ConstraintPosition(float[] constraintData)
    {
        int i = Mathf.FloorToInt(constraintData[0]);
        Vector3 a = subTriangles[i];        //Fetch vertices
        Vector3 b = subTriangles[i + 1];
        Vector3 c = subTriangles[i + 2];
        return (a + constraintData[1] * (b - a) + constraintData[2] * (c - a));
    }

    public override List<DCGElement> Extrude()
    {
        List<Point> corners = new List<Point>();
        List<Point> eCorners = new List<Point>();

        List<Point> ePoints = new List<Point>();
        List<Edge> eEdges = new List<Edge>();
        List<Face> eFaces = new List<Face>();
        List<DCGElement> eElems = new List<DCGElement>();

        eFaces.Add(this);
        eElems.Add(this);

        foreach (Edge e in edges)
        {
            corners.Add(e.points[0]);
            eCorners.Add(new Point(e.points[0].position));
            eElems.Add(e);
            eElems.Add(e.points[0]);
        }

        int i = 0;
        foreach (Edge e in edges)
        {
            List<Point> edgePoints = new List<Point>();
            int j = 0;
            foreach (Point p in e.points)
            {
                if (j < e.points.Count - 1)
                    ePoints.Add(p);


                if (j == 0)
                    edgePoints.Add(eCorners[i]);
                else if (j == e.points.Count - 1)
                    edgePoints.Add(eCorners[(i + 1) % eCorners.Count]);
                else
                    edgePoints.Add(new Point(p.position));
                ++j;
            }

            edgePoints.Reverse();
            foreach (Point p in edgePoints)
                eElems.Add(p);
            Edge eEdge = new Edge(edgePoints, e.isLoop);
            eEdges.Add(eEdge);

            List<Edge> faceEdges = new List<Edge>();
            faceEdges.Add(eEdge);
            faceEdges.Add(new Edge(eEdge.points[eEdge.points.Count - 1], e.points[0]));
            faceEdges.Add(e);
            faceEdges.Add(new Edge(e.points[e.points.Count - 1], eEdge.points[0]));
            Face nFace = new Face(faceEdges);
            eFaces.Add(nFace);
            
            ++i;
        }

        eEdges.Reverse();
        foreach (Edge e in eEdges)
            eElems.Add(e);
        Face gFace = new Face(eEdges);
        eFaces.Add(gFace);
        eElems.Add(new Solid(eFaces));
        eElems.Add(gFace);

        return eElems;
    }

    public void updateAwful()
    {
        Vector3 avgNormal = getNormal();
        /*if (!normalConfident)
        {
            isAwful = true;
            return;
        }*/
        List<Point> points = GetPoints();
        Vector2[] proj = new Vector2[points.Count];     //projected points
        Vector3 yVec = points[1].position - points[0].position;
        for (int i = 0; i < points.Count; ++i)
        {
            Vector3 proj3 = Vector3.ProjectOnPlane(points[i].position, avgNormal);
            Vector3 projY = Vector3.Project(proj3, yVec);
            proj[i] = new Vector2((proj3 - projY).magnitude, projY.magnitude);
        }

        for (int i = 0; i < points.Count; ++i)
        {
            Vector2 a = proj[i];
            Vector2 b = proj[(i + 1) % points.Count];
            Vector2 ir = new Vector2(a.y, -a.x);
            for (int j = i + 1; j < points.Count; ++j)
            {
                Vector2 c = proj[j];
                Vector2 d = proj[(i + 1) % points.Count];
                if (Mathf.Sign(Vector2.Dot(c - a, ir)) != Mathf.Sign(Vector2.Dot(d - a, ir)))
                {
                    Vector2 jr = new Vector2(c.y, -c.x);
                    if (Mathf.Sign(Vector2.Dot(a - c, jr)) != Mathf.Sign(Vector2.Dot(b - c, jr)))
                    {
                        isAwful = true;
                        return;
                    }
                }
            }
        }
        isAwful = false;
    }

    public virtual void updateMesh()
    {
        mesh = new Mesh();
        subTriangles = new List<Vector3>();
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3 avgNormal = getNormal();
        Vector3 u = Vector3.Cross(avgNormal, edges[1].points[1].position - edges[1].points[0].position);
        Vector3 v = Vector3.Cross(u, avgNormal);

        Vector3 lpos = edges[edges.Count - 1].points[edges[edges.Count - 1].points.Count - 1].position; //Gomenasai
        foreach (Edge e in edges)
            foreach (Point p in e.points)
            {
                if (p.position != lpos)
                {
                    verts.Add(p.position);
                    normals.Add(avgNormal);
                    uvs.Add(new Vector2(Vector3.Project(p.position, u).magnitude, Vector3.Project(p.position, v).magnitude));
                }
                lpos = p.position;
            }

        List<int> tris;
        if (verts.Count == 0) return;
        tris = GeometryUtil.smartTriangulate(verts, avgNormal);

        //mirror verts
        int vertCount = verts.Count;
        for (int i = 0; i < vertCount; ++i)
        {
            verts.Add(verts[i]);
            normals.Add(-avgNormal);
            uvs.Add(uvs[i]);
        }

        //mirror triangles
        int triCount = tris.Count;
        for (int i = 0; i < triCount; i += 3)
        {
            tris.Add(tris[i] + vertCount);
            tris.Add(tris[i + 2] + vertCount);
            tris.Add(tris[i + 1] + vertCount);
            subTriangles.Add(verts[tris[i] + vertCount]);
            subTriangles.Add(verts[tris[i + 1] + vertCount]);
            subTriangles.Add(verts[tris[i + 2] + vertCount]);
        }

        mesh.SetVertices(verts);
        mesh.SetNormals(normals);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }

    public override List<Point> GetPoints()
    {
        List<Point> points = new List<Point>();
        foreach (Edge e in edges)
        {
            points.AddRange(e.points);
        }
        return points.Distinct().ToList();
    }

    public Vector3 getNormal()
    {
        List<Point> points = GetPoints();
        Vector3 prevPos = points[points.Count - 1].position;
        Vector3 prev = prevPos - points[points.Count - 2].position;
        Vector3 norm = new Vector3(0, 0, 0);
        foreach (Point p in points)
        {
            Vector3 next = p.position - prevPos;
            Vector3 cross = Vector3.Cross(prev, next);
            if (Vector3.Dot(cross, norm) < 0)
            {
                cross = -cross;
                normalConfident = false;
            }
            norm += cross;
            prev = next;
            prevPos = p.position;
        }

        return norm.normalized;
    }

    public List<Face> GetConnectedFaces(int depth)
    {
        int id = DCGBase.nextMoveID();
        List<Face> found = new List<Face>();
        TraverseFaces(found, depth, id);
        return found;
    }

    protected void TraverseFaces(List<Face> found, int depth, int moveID)
    {
        found.Add(this);    //Add this to found faces
        lastMoveID = moveID;
        if (depth == 0) //If we're at the bottom of the depth, don't search any further
            return;

        foreach (Edge e in edges)
        {
            Debug.Log("adjacent: " + e.faces.Count);
            foreach (Face w in e.faces)
                if (w.lastMoveID != moveID)
                    w.TraverseFaces(found, depth - 1, moveID);
        }

        return;
    }

    public override void Lock()
    {
        foreach (Edge e in edges)
        {
            if (!e.isLocked)
                e.Lock();
        }
        isLocked = true;
    }
    public override void Unlock()
    {
        foreach (Edge e in edges)
        {
            if (e.isLocked)
                e.Unlock();
        }
        isLocked = false;
    }
    public override DCGElement Copy(int moveId = -1)
    {
        Face copy = (Face)lastCopyMade;
        if (this.lastMoveID != moveId)
        {
            List<Edge> cEdges = new List<Edge>();
            foreach (Edge e in edges)
            {
                Edge edgeCopy = (Edge)e.Copy(moveId);
                if (edgeCopy != null && !cEdges.Contains(edgeCopy))
                {
                    cEdges.Add(edgeCopy);
                }
            }
            cEdges = cEdges.Distinct().ToList();          
            copy = new Face(cEdges);
        }
        lastCopyMade = copy;
        return copy;
    }
    public override bool ParentSelected()
    {
        foreach (Solid s in solids)
            if (DCGBase.sElements.Contains(s))
                return true;
        return false;
    }
    private float _PlanarEpsilon = .001f;
    public bool isPlanar()
    {
        List<Point> points = GetPoints();
        Vector3 testNormal = Vector3.Cross(points[2].position - points[0].position, points[1].position - points[0].position);

        for (int i = 3; i < points.Count; ++i) if (Vector3.Dot(points[i].position - points[0].position, testNormal) > _PlanarEpsilon)
                return false;
        return true;
    }
    public override List<DCGElement> GetParents()
    {
        List<DCGElement> elems = new List<DCGElement>();
        foreach (Solid s in solids)
            elems.Add(s);
        return elems;
    }
    public override List<DCGElement> GetChildren()
    {
        List<DCGElement> elems = new List<DCGElement>();
        foreach(Edge e in edges)
        {
            elems.Add(e);
            elems.AddRange(e.GetChildren());
        }
        return elems.Distinct().ToList();
    }
}
