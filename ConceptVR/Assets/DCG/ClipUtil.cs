using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*List of yet unhandled corner cases
    2D fully contained clipping
 */

public class ClipUtil {

    //To whomever needs to read this code....i am so sorry. 
    //This does CSG, Unity might have their built in CSG done, go use it if you want instead.
    public enum ClipMode { Add, Subtract, Intersect };
    public struct CPoint
    {
        public Vector3 position;
        public int id;

        public CPoint(Point p)
        {
            position = p.position;
            id = p.elementID;
        }

        public CPoint(Vector3 position)
        {
            this.position = position;
            id = -1;
        }

        public void SetId(int value) {this.id = value;}
    }

    public struct CFace
    {
        public int[] points;
        public CSolid solid;

        public Vector3[] GetVerts() {
            Vector3[] verts = new Vector3[points.Length];
            for (int i = 0; i < points.Length; ++i) {
                verts[i] = solid.vertices[points[i]].position;
            }
            return verts;
        }

        public void debugDraw(Color c) {
            for(int i = 0; i < points.Length; ++i) {
                int j = (i+1)%points.Length;
                Debug.DrawLine(solid.vertices[points[i]].position, solid.vertices[points[j]].position, c, 1000, true);
            }
        }
    }

    public struct CSolid
    {
        public List<CFace> faces;
        public List<CPoint> vertices;

        public bool Contains(Vector3 v)
        {
            int fCount = 0;

            foreach (CFace f in faces)
            {
                int eCount = 0;
                for(int i = 0; i < f.points.Length; ++i)
                {
                    Vector3 a = vertices[f.points[i]].position;
                    Vector3 b = vertices[f.points[(i+1) % f.points.Length]].position;
                    if (a.y != b.y &&   //ab Not parallel to detector
                        (a.y <= v.y && v.y < b.y || b.y <= v.y && v.y < a.y) && //v in between a and b on y axis
                        v.x <= a.x + (v.y - a.y) * (b.x - a.x) / (b.y - a.y))   //v left of a on x axis
                        ++eCount;
                }

                if (eCount % 2 == 1) {
                    Vector3 a = vertices[f.points[f.points.Length - 1]].position - vertices[f.points[0]].position;
                    Vector3 b = vertices[f.points[1]].position - vertices[f.points[0]].position;
                    Vector3 c = Vector3.Cross(a, b);
                    if (Vector3.Dot(c, Vector3.forward) < 0)
                        c = -c;
                    Vector3 u = v - vertices[f.points[0]].position;
                    if (c.z != 0 && Vector3.Dot(u, c) >= 0)
                        ++fCount;
                }
            }

            return fCount % 2 == 1;
        }

        public CFace MapFace(Vector3[] positions) {
            CFace f = new CFace();
            f.solid = this;
            f.points = new int[positions.Length];
            for(int i = 0; i < positions.Length; ++i) {
                bool found = false;
                for(int j = 0; j < vertices.Count; ++j)
                    if (vertices[j].position == positions[i]){
                        f.points[i] = j;
                        found = true;
                        break;
                    }
                if (!found) {
                    f.points[i] = vertices.Count;
                    vertices.Add(new CPoint(positions[i]));
                }
            }
            return f;
        }
    }

    public static Solid Clip(Solid A, Solid B, ClipMode mode)
    {
        List<CFace> Ai, Ax, Ao, Bi, Bx, Bo;
        CSolid Ac = ReduceSolid(A);
        CSolid Bc = ReduceSolid(B);
        //Divvy(Ac, Bc, out Ai, out Ax, out Ao);
        //Divvy(Bc, Ac, out Bi, out Bx, out Bo);
        Ai = new List<CFace>();
        Ao = new List<CFace>();
        Bi = new List<CFace>();
        Bo = new List<CFace>();


        foreach (CFace a in Ac.faces) {
            Vector3[] p = PolyTransform(a.GetVerts(), a);
            List<List<Vector3>> cross = CrossSection(Bc, a);
            foreach(List<Vector3> ql in cross) {
                Vector3[] q = ql.ToArray();
                List<Vector3[]> pi, po;
                Clip2D(p, q, out pi, out po);
                foreach (Vector3[] poly in pi)
                    Ai.Add(Ac.MapFace(DePolyTransform(poly, a)));
                foreach (Vector3[] poly in po)
                    Ao.Add(Ac.MapFace(DePolyTransform(poly, a)));
            }
            if (cross.Count == 0) 
                if (B.ContainsPoint(Ac.vertices[a.points[0]].position))
                    Ai.Add(a);
                else
                    Ao.Add(a);
        }
        
        foreach (CFace b in Bc.faces) {
            Vector3[] p = PolyTransform(b.GetVerts(), b);
            List<List<Vector3>> cross = CrossSection(Ac, b);
            foreach(List<Vector3> ql in cross) {
                Vector3[] q = ql.ToArray();
                List<Vector3[]> pi, po;
                Clip2D(p, q, out pi, out po);
                foreach (Vector3[] poly in pi)
                    Bi.Add(Bc.MapFace(DePolyTransform(poly, b)));
                foreach (Vector3[] poly in po)
                    Bo.Add(Bc.MapFace(DePolyTransform(poly, b)));
            }
            if (cross.Count == 0) 
                if (A.ContainsPoint(Bc.vertices[b.points[0]].position))
                    Bi.Add(b);
                else
                    Bo.Add(b);
        }


        //Set up merged CSolid
        CSolid Cc;
        Cc.vertices = new List<CPoint>();
        Cc.faces = new List<CFace>();
        int ACount = Ac.vertices.Count;
        Cc.vertices.AddRange(Ac.vertices);
        Cc.vertices.AddRange(Bc.vertices);

        //find points that are probably the same
        int[] map = new int[Cc.vertices.Count];
        float epsilon = 0.005f;
        for (int i = 0; i < Cc.vertices.Count; ++i)
            map[i] = -1;
        for (int i = Cc.vertices.Count-1; i >= 0 ; --i) {
            for (int j = 0; j <= i; ++j) {
                if (Vector3.Distance(Cc.vertices[i].position, Cc.vertices[j].position) < epsilon) {
                    if (Cc.vertices[i].id != -1)
                        Cc.vertices[j].SetId(Cc.vertices[i].id);
                    map[i] = j;
                    break;
                }
            }
        }

        //Add verts to merged CSolid
        if (mode == ClipMode.Subtract) {
            foreach (CFace f in Ao) 
                for (int i = 0; i < f.points.Length; ++i)
                    f.points[i] = map[f.points[i]];
            Cc.faces.AddRange(Ao);
            foreach (CFace f in Bi) 
                for (int i = 0; i < f.points.Length; ++i)
                    f.points[i] = map[f.points[i] + ACount];
            Cc.faces.AddRange(Bi);
        } else if (mode == ClipMode.Add) {
            //Merge A and B into C, using faces Ao, Bi, adding to point indices of Bi respectively (TODO)
        } else if (mode == ClipMode.Intersect) {
            //Merge A and B into C, using faces Ai, Bi, adding to point indices of Bi respectively (TODO)
        }

        //Find used vertices
        bool[] used = new bool[Cc.vertices.Count];
        foreach (CFace f in Cc.faces)
            foreach (int i in f.points)
                used[i] = true;

        //Map duplicate points, create new points, delete unused points
        Point[] points = new Point[Cc.vertices.Count];  //Sparse Array
        Dictionary<int, Edge>[] edgeTo = new Dictionary<int, Edge>[Cc.vertices.Count];  //Sparse grid
        for (int i = 0; i < Cc.vertices.Count; ++i) {
            if (used[i]) {
                if(map[i] == i) {
                    points[i] = Cc.vertices[i].id == -1 ? new Point(Cc.vertices[i].position) : DCGBase.all[Cc.vertices[i].id] as Point;
                    Cc.vertices[i].SetId(points[i].elementID);
                    edgeTo[i] = new Dictionary<int, Edge>();
                }
            } else {
                if (Cc.vertices[i].id != -1) {
                    DCGBase.all[Cc.vertices[i].id].Remove();
                }
            }
        }

        //Nuke A and B
        foreach (Point p in A.getPoints())
            for (int i = p.edges.Count-1; i >= 0; --i)
                p.edges[i].Remove();
                
        foreach (Point p in B.getPoints())
            for (int i = p.edges.Count-1; i >= 0; --i)
                p.edges[i].Remove();

        //Create new edges/faces in DCG
        Face[] faces = new Face[Cc.faces.Count];
        for (int i = 0; i < Cc.faces.Count; ++i) {
            CFace f = Cc.faces[i];
            Edge[] edges = new Edge[f.points.Length];

            Debug.Log(f.points.Length);
            for(int j = 0; j < f.points.Length; ++j) {
                int k = (j + 1) % f.points.Length;
                int pk = f.points[k];
                int pj = f.points[j];
                bool kcj = edgeTo[pk].ContainsKey(pj);
                bool jck = edgeTo[pj].ContainsKey(pk);
                if (jck)
                {
                    edges[j] = edgeTo[pj][pk];
                    if (!kcj)
                        edgeTo[pk].Add(pj, edges[j]);
                }
                else if (kcj)
                {
                    edges[j] = edgeTo[pk][pj];
                    if (!jck)
                        edgeTo[pj].Add(pk, edges[j]);
                }
                else
                {
                    edges[j] = new Edge(points[pj], points[pk]);
                    //if (!jck)
                    edgeTo[pj].Add(pk, edges[j]);
                    //if (!kcj)
                    edgeTo[pk].Add(pj, edges[j]);
                }
                //Debug.DrawLine(edges[j].points[0].position, edges[j].points[1].position, new Color(Random.value, Random.value, Random.value), 1000, false);
            }
            //Debug.Log(edges.Length);
            faces[i] = new Face(new List<Edge>(edges));
        }

        //Create new solid from C
        return new Solid(new List<Face>(faces));
        //return new Solid();

        //TODO eventually, if we have time:
            //Relate generated Faces and Edges to existing Faces and Edges, in order to reduce new objects being created or to copy materials/etc.
    }

    public static Vector3[] PolyTransform(Vector3[] V, CFace poly) {
        Vector3 a = poly.solid.vertices[poly.points[0]].position;
        Vector3 b = poly.solid.vertices[poly.points[1]].position;
        Vector3 c = poly.solid.vertices[poly.points[poly.points.Length-1]].position;
        Vector3 X = (b - a).normalized;
        Vector3 Y = Vector3.Cross(X, c-a).normalized;
        Vector3 Z = Vector3.Cross(Y, X);

        Vector3[] U = new Vector3[V.Length];

        for (int i = 0; i < V.Length; ++i)
            U[i] = new Vector3( Vector3.Dot(V[i]-a, X),
                                Vector3.Dot(V[i]-a, Y),
                                Vector3.Dot(V[i]-a, Z)  );
        
        return U;
    }

    public static Vector3[] DePolyTransform(Vector3[] U, CFace poly) {
        Vector3 a = poly.solid.vertices[poly.points[0]].position;
        Vector3 b = poly.solid.vertices[poly.points[1]].position;
        Vector3 c = poly.solid.vertices[poly.points[poly.points.Length-1]].position;
        Vector3 X = (b - a).normalized;
        Vector3 Y = Vector3.Cross(X, c-a).normalized;
        Vector3 Z = Vector3.Cross(Y, X);

        Vector3[] V = new Vector3[U.Length];

        for (int i = 0; i < U.Length; ++i)
            V[i] = a + U[i].x * X + U[i].y * Y + U[i].z * Z;
        
        return V;
    }

    public static List<List<Vector3>> CrossSection(CSolid B, CFace a) {
        Vector3[] bTrans = new Vector3[B.vertices.Count];
        Vector3[] aTrans = new Vector3[a.points.Length];
        
        for(int i = 0; i < a.points.Length; ++i)
            aTrans[i] = a.solid.vertices[a.points[i]].position;
        for (int i = 0; i < B.vertices.Count; ++i)
            bTrans[i] = B.vertices[i].position;
        
        aTrans = PolyTransform(aTrans, a);
        bTrans = PolyTransform(bTrans, a);

        List<List<Vector3>> edges = new List<List<Vector3>>();

        float epsilonSquared = 0.00001f;
        
        foreach (CFace b in B.faces) {
            List<Vector3> intersections = new List<Vector3>();
            for (int i = 0; i < b.points.Length; ++i) {
                int j = (i + 1) % b.points.Length;
                int pi = b.points[i];
                int pj = b.points[j];
                if (Mathf.Sign(bTrans[pi].y) != Mathf.Sign(bTrans[pj].y)) {
                    float t = -bTrans[pi].y / (bTrans[pj].y - bTrans[pi].y);
                    intersections.Add(bTrans[pi] + (bTrans[pj] - bTrans[pi]) * t);
                }
            }
            if (intersections.Count > 1)
                for (int i = intersections.Count-1; i >= 0; --i)
                    for (int j = 0; j < i; ++j)
                        if ((intersections[j] - intersections[i]).sqrMagnitude <= epsilonSquared)
                        {
                            intersections.RemoveAt(i);
                            i = Mathf.Min(i, intersections.Count-1);
                        }

            if (intersections.Count > 1) {
                Vector3 edgeDir = intersections[intersections.Count-1] - intersections[0];
                intersections.Sort((x, y) => (int)Mathf.Sign(Vector3.Dot(y-x, edgeDir)));
                for(int i = 0; i < intersections.Count/2; ++i){
                    List<Vector3> edge = new List<Vector3>();
                    edge.Add(intersections[i*2]); edge.Add(intersections[i*2+1]);
                    edges.Add(edge);
                }
            } else if (intersections.Count == 1) {
                List<Vector3> edge = new List<Vector3>();
                edge.Add(intersections[0]);
                edges.Add(edge);
            }
        }

        for (int i = edges.Count-1; i >= 0; --i) {
            for (int j = 0; j < edges.Count; ++j) {
                if (i == j) continue;
                if ((edges[j][edges[j].Count-1] - edges[i][0]).sqrMagnitude <= epsilonSquared) {
                    for (int k = 1; k < edges[i].Count; ++k)
                        edges[j].Add(edges[i][k]);
                    edges[i].Clear();
                    edges.RemoveAt(i);
                    break;
                } else if ((edges[j][edges[j].Count-1] - edges[i][edges[i].Count-1]).sqrMagnitude <= epsilonSquared) {
                    for (int k = edges[i].Count-2; k >= 0; --k)
                        edges[j].Add(edges[i][k]);
                    edges[i].Clear();
                    edges.RemoveAt(i);
                    break;
                } else if ((edges[j][0] - edges[i][edges[i].Count-1]).sqrMagnitude <= epsilonSquared) {
                    for (int k = edges[i].Count-2; k >= 0 ; --k)
                        edges[j].Insert(0, edges[i][k]);
                    edges[i].Clear();
                    edges.RemoveAt(i);
                    break;
                } else if ((edges[j][0] - edges[i][0]).sqrMagnitude <= epsilonSquared) {
                    for (int k = 1; k < edges[i].Count-1; ++k)
                        edges[j].Insert(0, edges[i][k]);
                    edges[i].Clear();
                    edges.RemoveAt(i);
                    break;
                }
            }
        }

        for (int i = edges.Count-1; i >= 0; --i) {
            if (edges[i].Count < 3)
                edges.RemoveAt(i);
            else if ((edges[i][edges[i].Count-1] - edges[i][0]).sqrMagnitude <= epsilonSquared) {
                edges[i].RemoveAt(edges[i].Count-1);
            }
        }

        return edges;
    }

    public static CSolid ReduceSolid(Solid A)
    {
        CSolid s = new CSolid();
        s.faces = new List<CFace>();
        s.vertices = new List<CPoint>();
        
        foreach (Face a in A.faces)
        {
            if (a.isPlanar())
            {
                CFace ca = new CFace();
                List<Point> aPoints = a.GetPoints();
                ca.points = new int[aPoints.Count];
                ca.solid = s;
                for(int i = 0; i < aPoints.Count; ++i)
                {
                    int index = s.vertices.FindIndex(x => x.position == aPoints[i].position);
                    if (index == -1)
                    {
                        s.vertices.Add(new CPoint(aPoints[i]));
                        ca.points[i] = s.vertices.Count - 1;
                    }
                    else
                        ca.points[i] = index;
                }
                s.faces.Add(ca);
            }
            else
            {
                List<Point> aPoints = a.GetPoints();

                for (int i = 0; i < aPoints.Count; ++i)
                {
                    int index = s.vertices.FindIndex(x => x.position == aPoints[i].position);
                    if (index == -1)
                        s.vertices.Add(new CPoint(aPoints[i]));
                }

                for (int i = 0; i < a.subTriangles.Count; i += 3)
                {
                    CFace ct = new CFace();
                    ct.solid = s;
                    ct.points = new int[] {
                        s.vertices.FindIndex(x => x.position == a.subTriangles[i]),
                        s.vertices.FindIndex(x => x.position == a.subTriangles[i+1]),
                        s.vertices.FindIndex(x => x.position == a.subTriangles[i+2])    };
                    s.faces.Add(ct);
                }
            }
        }

        return s;
    }

    
    /*static void Divvy(CSolid A, CSolid B, out List<CFace> Ai, out List<CFace> Ax, out List<CFace> Ao)
    {
        bool[] vIn = new bool[A.vertices.Count];
        for (int i = 0; i < A.vertices.Count; ++i) {
            vIn[i] = B.Contains(A.vertices[i].position);
            //Debug.DrawLine(A.vertices[i].position, A.vertices[i].position + Vector3.one/20f, vIn[i] ? Color.green : Color.red, 1000);
        }

        Ai = new List<CFace>();
        Ax = new List<CFace>();
        Ao = new List<CFace>();

        foreach (CFace f in A.faces)
        {
            
            bool allIn = true, noneIn = true;
            foreach (int n in f.points)
            {
                allIn = allIn && vIn[n];
                noneIn = noneIn && !vIn[n];
            }

            if (allIn) {
                Ai.Add(f);
                //f.debugDraw(Color.black);
            }
            else if (noneIn) {
                Ao.Add(f);
                //f.debugDraw(Color.white);
            }
            else {
                Ax.Add(f);
                //f.debugDraw(Color.green);
            }
        }

        return;
    }*/

    private struct Clip2DIntersection {
        public Vector3 position;
        public int ip;
        public int iq;
        public bool inbound;

        public Clip2DIntersection(Vector3 position, int ip, int iq, bool outIn) {
            this.position = position;
            this.ip = ip;
            this.iq = iq;
            this.inbound = outIn;
        }
    }

    private class PolyPoint {
        public Vector3 position;
        public bool visited;
        
        public bool isIntersection;
        public int ip;
        public int iq;
        public bool inbound;

        public PolyPoint(Vector3 position) {
            this.position = position;
            this.isIntersection = false;
        }
    }

    public static void Clip2D(Vector3[] P, Vector3[] Q, out List<Vector3[]> Pi, out List<Vector3[]> Po) {
        P = Clockwise(P);
        Q = Clockwise(Q);
        int i, j;

        bool[] pIn = new bool[P.Length];

        List<Clip2DIntersection> intersections = new List<Clip2DIntersection>();

        for (i = 0; i < P.Length; ++i) {
            for (j = 0; j < Q.Length; ++j) {
                int iN = (i+1) % P.Length;
                int jN = (j+1) % Q.Length;
                float interA = IntersectionScalar(P[i], P[iN]-P[i], Q[j], Q[jN]-Q[j]);
                float interB = IntersectionScalar(Q[j], Q[jN]-Q[j], P[i], P[iN]-P[i]);
                if (0 <= interA && interA <= 1 && 0 <= interB && interB <= 1) {
                    bool outIn = Cross2(Q[jN]-Q[j], P[i]-Q[j]) >= 0 && Cross2(Q[jN]-Q[j], P[iN]-P[i]) < 0;
                    intersections.Add(new Clip2DIntersection(P[i] + (P[iN]-P[i]) * interA, i, j, outIn));
                }
            }
        }
        

        List<PolyPoint> Pann = new List<PolyPoint>(), Qann = new List<PolyPoint>(), interPoints = new List<PolyPoint>(); //P and Q annotated with intersections

        for (i = 0; i < intersections.Count; ++i) {
            PolyPoint p = new PolyPoint(intersections[i].position);
            p.inbound = intersections[i].inbound;
            p.isIntersection = true;
            interPoints.Add(p);
        }

        i = 0;
        while (i < P.Length) {
            int iN = (i+1) % P.Length;
            PolyPoint p = new PolyPoint(P[i]);
            p.ip = Pann.Count;
            Pann.Add(p);

            List<Clip2DIntersection> segInter = intersections.FindAll(x => x.ip == i);
            segInter.Sort((x,y) => (int)Mathf.Sign( Vector3.Dot(P[iN] - P[i], x.position - P[i]) - Vector3.Dot(P[iN] - P[i], y.position - P[i]) ));

            foreach (Clip2DIntersection inter in segInter) {
                PolyPoint interP = interPoints.Find(x => x.position == inter.position);
                interP.ip = Pann.Count;
                Pann.Add(interP);
            }
            ++i;
        }

        i = 0;
        while (i < Q.Length) {
            int iN = (i+1) % Q.Length;
            PolyPoint q = new PolyPoint(Q[i]);
            q.iq = Qann.Count;
            Qann.Add(q);

            List<Clip2DIntersection> segInter = intersections.FindAll(x => x.iq == i);
            segInter.Sort((x,y) => (int)Mathf.Sign( Vector3.Dot(Q[iN] - Q[i], x.position - Q[i]) - Vector3.Dot(Q[iN] - Q[i], y.position - Q[i]) ));

            foreach (Clip2DIntersection inter in segInter) {
                PolyPoint interP = interPoints.Find(x => x.position == inter.position);
                interP.iq = Qann.Count;
                Qann.Add(interP);
            }
            ++i;
        }

        Pi = new List<Vector3[]>();
        Po = new List<Vector3[]>();

        foreach (PolyPoint p in interPoints)
            if (p.inbound && !p.visited) {
                List<PolyPoint> res = BuildClipPoly(p, Pann, Qann, 'i');
                Pi.Add(new Vector3[res.Count]);
                for(i = 0; i < res.Count; ++i)
                    Pi[Pi.Count-1][i] = res[i].position;
            }
                
        foreach (PolyPoint p in interPoints)
            p.visited = false;
        foreach (PolyPoint p in interPoints)
            if (p.inbound && !p.visited) {
                List<PolyPoint> res = BuildClipPoly(p, Pann, Qann, 'o');
                Po.Add(new Vector3[res.Count]);
                for(i = 0; i < res.Count; ++i)
                    Po[Po.Count-1][i] = res[i].position;
            }
    }

    //mode 'o' for P outside Q, mode 'i' for P inside Q
    static List<PolyPoint> BuildClipPoly(PolyPoint start, List<PolyPoint> P, List<PolyPoint> Q, char mode) {
        List<PolyPoint> poly = new List<PolyPoint>();
        int i = start.ip;
        PolyPoint curr = start;
        List<PolyPoint> currList = P;
        int loopCount = 0;
        int loopMax = P.Count + Q.Count + 2;
        do {
            curr.visited = true;
            poly.Add(curr);
            bool inter = curr.isIntersection;
            bool n = curr.inbound;
            bool p = currList == P;
            bool o = mode == 'o';

            if (!o && (!inter && !p || inter && !n)) {  //Determine the next node to visit
                i = (curr.iq + 1) % Q.Count;
                currList = Q;
            } else if (o && (!inter && !p || inter && n)) {
                i = (curr.iq + Q.Count - 1) % Q.Count;
                currList = Q;
            } else {
                i = (curr.ip + 1) % P.Count;
                currList = P;
            }

            ++loopCount;

            curr = currList[i];
        } while (curr != start && loopCount < loopMax);
        
        return poly;
    }

    static float Cross2(Vector3 a, Vector3 b) {
        return a.x*b.z - b.x*a.z;
    }
    static float IntersectionScalar(Vector3 a, Vector3 aDir, Vector3 b, Vector3 bDir) {
        float dx = Cross2(aDir, bDir);
        Vector3 ba = b-a;
        return Cross2(ba, bDir)/dx;
    }

    static Vector3[] Widdershins(Vector3[] P) {
        float area = Area(P);
        if (area < 0) {
            for (int i = 0; i < P.Length/2; ++i){
                Vector3 temp = P[i];
                P[i] = P[P.Length-i-1];
                P[P.Length-i-1] = temp;
            }
        }
        return P;
    }

    static Vector3[] Clockwise(Vector3[] P) {
        float area = Area(P);
        if (area > 0) {
            for (int i = 0; i < P.Length/2; ++i){
                Vector3 temp = P[i];
                P[i] = P[P.Length-i-1];
                P[P.Length-i-1] = temp;
            }
        }
        return P;
    }

    static float Area(Vector3[] P) {
        float area = 0;
        for (int i = 0; i < P.Length; ++i) {
            int j = (i+1) % P.Length;
            area += (P[i].x*P[j].z - P[j].x*P[i].z);
        }
        return area/2;
    }
}
