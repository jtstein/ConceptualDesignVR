using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeometryUtil
{
    //To anyone reading this Im sorry you have to read this -L
    public static Mesh icoSphere2 = GenerateSphereMesh(2);
    public static Mesh icoSphere3 = GenerateSphereMesh(3);
    public static Mesh icoSphere4 = GenerateSphereMesh(4);

    public static Mesh cylinder8 = GenerateCylinderMesh(8);
    public static Mesh cylinder16 = GenerateCylinderMesh(16);
    public static Mesh cylinder32 = GenerateCylinderMesh(32);
    public static Mesh cylinder64 = GenerateCylinderMesh(64);



    public static void MinimizeMesh(Mesh mesh)
    {
        List<Vector3> pos = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        mesh.GetVertices(pos);
        mesh.GetNormals(normals);
        int[] tris = mesh.GetTriangles(0);

        int[] map = new int[pos.Count];
        List<Vector3> nPosList = new List<Vector3>();

        for (int i = 0; i < pos.Count; ++i)
        {
            bool match = false;
            int j = 0;
            foreach (Vector3 v in nPosList)
            {
                if (pos[i] == v)
                {
                    map[i] = j;
                    match = true;
                }
                ++j;
            }

            if (!match)
            {
                nPosList.Add(pos[i]);
                map[i] = nPosList.Count - 1;
            }
        }

        for (int i = 0; i < tris.Length; ++i)
        {
            tris[i] = map[tris[i]];
        }

        //mesh.SetNormals();    //TODO: set mesh stuff, do normals
    }

    static void subdivide(List<Vector3> verts, List<int> tris, int v1, int v2, int v3, int depth)
    {
        if (depth > 0)
        {
            int b = verts.Count;
            //Debug.Log(b + " " + v1);
            verts.Add((verts[v1] + verts[v2]).normalized);
            verts.Add((verts[v2] + verts[v3]).normalized);
            verts.Add((verts[v3] + verts[v1]).normalized);

            subdivide(verts, tris, v1, b, b + 2, depth - 1);
            subdivide(verts, tris, b, v2, b + 1, depth - 1);
            subdivide(verts, tris, b + 2, b + 1, v3, depth - 1);
            subdivide(verts, tris, b, b + 1, b + 2, depth - 1);
        }
        else
        {
            tris.Add(v1);
            tris.Add(v2);
            tris.Add(v3);
        }
    }

    public static Mesh GenerateSphereMesh(int depth)
    {
        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        List<int> tris = new List<int>(20 * Mathf.FloorToInt(Mathf.Pow(4, depth)));
        List<Vector3> verts = new List<Vector3>(12 * Mathf.FloorToInt(Mathf.Pow(2, depth)));

        verts.Add(new Vector3(-1, t, 0).normalized);
        verts.Add(new Vector3(1, t, 0).normalized);
        verts.Add(new Vector3(-1, -t, 0).normalized);
        verts.Add(new Vector3(1, -t, 0).normalized);

        verts.Add(new Vector3(0, -1, t).normalized);
        verts.Add(new Vector3(0, 1, t).normalized);
        verts.Add(new Vector3(0, -1, -t).normalized);
        verts.Add(new Vector3(0, 1, -t).normalized);

        verts.Add(new Vector3(t, 0, -1).normalized);
        verts.Add(new Vector3(t, 0, 1).normalized);
        verts.Add(new Vector3(-t, 0, -1).normalized);
        verts.Add(new Vector3(-t, 0, 1).normalized);

        subdivide(verts, tris, 0, 11, 5, depth);
        subdivide(verts, tris, 0, 5, 1, depth);
        subdivide(verts, tris, 0, 1, 7, depth);
        subdivide(verts, tris, 0, 7, 10, depth);
        subdivide(verts, tris, 0, 10, 11, depth);

        subdivide(verts, tris, 1, 5, 9, depth);
        subdivide(verts, tris, 5, 11, 4, depth);
        subdivide(verts, tris, 11, 10, 2, depth);
        subdivide(verts, tris, 10, 7, 6, depth);
        subdivide(verts, tris, 7, 1, 8, depth);

        subdivide(verts, tris, 3, 9, 4, depth);
        subdivide(verts, tris, 3, 4, 2, depth);
        subdivide(verts, tris, 3, 2, 6, depth);
        subdivide(verts, tris, 3, 6, 8, depth);
        subdivide(verts, tris, 3, 8, 9, depth);

        subdivide(verts, tris, 4, 9, 5, depth);
        subdivide(verts, tris, 2, 4, 11, depth);
        subdivide(verts, tris, 6, 2, 10, depth);
        subdivide(verts, tris, 8, 6, 7, depth);
        subdivide(verts, tris, 9, 8, 1, depth);

        Mesh m = new Mesh();
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.SetNormals(verts);
        m.RecalculateBounds();
        m.RecalculateTangents();

        return m;
    }

    public static Mesh GenerateCylinderMesh(int detail)
    {
        List<int> tris = new List<int>();
        List<Vector3> verts = new List<Vector3>();

        for (int i = 0; i < detail; ++i)
        {
            float theta = (float)i / detail * Mathf.PI * 2;
            verts.Add(new Vector3(Mathf.Cos(theta), -1, Mathf.Sin(theta)));
            verts.Add(new Vector3(Mathf.Cos(theta), 1, Mathf.Sin(theta)));

            int a = i * 2;
            int b = ((i + 1) % detail) * 2;
            tris.Add(a); tris.Add(b + 1); tris.Add(b);
            tris.Add(b + 1); tris.Add(a); tris.Add(a + 1);
            tris.Add(a); tris.Add(b); tris.Add(detail * 2);
            tris.Add(a + 1); tris.Add(b + 1); tris.Add(detail * 2 + 1);
        }

        verts.Add(new Vector3(0, -1, 0));
        verts.Add(new Vector3(0, 1, 0));

        Mesh m = new Mesh();
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.RecalculateNormals();
        m.RecalculateBounds();
        m.RecalculateTangents();

        return m;
    }


    //Not the worst I guess, 3D makes it make sense...ish
    public static List<int> mediocreTriangulate(List<Vector3> points)
    {
        int c = points.Count;
        List<int> triangles = new List<int>();
        bool[] consumed = new bool[c];
        int consumedCount = 0;

        int i = 0;
        int i1 = 1;
        int i2 = 2;
        while (consumedCount < c - 3)
        {
            i1 = (i + 1) % c;
            while (consumed[i1])
                i1 = (i1 + 1) % c;

            i2 = (i1 + 1) % c;
            while (consumed[i2])
                i2 = (i2 + 1) % c;

            triangles.Add(i); triangles.Add(i1); triangles.Add(i2);
            consumed[i1] = true;
            consumedCount++;
            i = i2;

        }


        i1 = (i + 1) % c;
        while (consumed[i1])
            i1 = (i1 + 1) % c;

        i2 = (i1 + 1) % c;
        while (consumed[i2])
            i2 = (i2 + 1) % c;

        triangles.Add(i); triangles.Add(i1); triangles.Add(i2);

        return triangles;
    }

    public static List<int> smartTriangulate(List<Vector3> points, Vector3 normal)
    {
        List<Vector2> p2 = planarize(points, normal, points[1]-points[0]);

        bool flip = area(p2) < 0;   //Ensure clockwise
        if (flip)
            for (int t = 0; t < p2.Count; ++t)
                p2[t] = new Vector2(p2[t].x, -p2[t].y);

        List<int> tri = new List<int>();

        List<bool> exc = new List<bool>();  //Already excluded points
        for (int a = 0; a < points.Count; ++a)
            exc.Add(false);
        int excCount = 0;

        int i = 0;
        int j, k;
        int runCount = 0;
        int maxRun = p2.Count*p2.Count; //It shouldn't be able to crash, but just in case...
        while (excCount < points.Count-3 && runCount < maxRun)
        {
            ++runCount;
            j = next(i, exc);
            k = next(j, exc);

            if (Cross2(p2[j] - p2[i], p2[k] - p2[i]) > 0)  //If this tri is clockwise
            {
                for (int v = 0; v < p2.Count; ++v)    //Verify this tri contains no other points
                {
                    float ci = Cross2(p2[j] - p2[i], p2[v] - p2[i]);
                    float cj = Cross2(p2[k] - p2[j], p2[v] - p2[j]);
                    float ck = Cross2(p2[i] - p2[k], p2[v] - p2[k]);

                    if (i != v && j != v && k != v && ci > 0 && cj > 0 && ck >= 0)
                        goto STBreak;
                }

                tri.Add(i); tri.Add(j); tri.Add(k);
                exc[j] = true;
                ++excCount;
            }
            STBreak:
            i = next(i, exc);
        }

        //Debug.Log(runCount + " " + excCount);

        j = next(i, exc);
        k = next(j, exc);
        i = next(k, exc);

        tri.Add(i); tri.Add(j); tri.Add(k);

        return tri;
    }

    private static int next(int i, List<bool> exclude)
    {
        int start = i;
        do
        {
            i = (i + 1) % exclude.Count;
        } while (i != start && exclude[i]);

        return i;
    }

    public static float Cross2(Vector2 u, Vector2 v)
    {
        return (u.x * v.y - u.y * v.x);
    }

    public static List<Vector2> planarize(List<Vector3> points, Vector3 normal, Vector3 up)
    {
        List<Vector2> planar = new List<Vector2>();
        Vector3 pUp = Vector3.ProjectOnPlane(up, normal).normalized;
        Vector3 pRight = Vector3.Cross(pUp, normal).normalized;

        foreach(Vector3 v in points)
        {
            Vector3 proj = Vector3.ProjectOnPlane(v, normal);
            planar.Add(new Vector2(
                Vector3.Project(proj, pUp).magnitude * Mathf.Sign(Vector3.Dot(proj, pUp)), 
                Vector3.Project(proj, pRight).magnitude * Mathf.Sign(Vector3.Dot(proj, pRight))
            ));
        }

        return planar;
    }

    public static float area(List<Vector2> points) {
        float area = 0;
        for (int i = 0; i < points.Count; ++i) {
            int j = (i+1) % points.Count;
            area += (points[i].x*points[j].y - points[j].x*points[i].y);
        }
        return area /= 2;
    }

    public enum MergeMode { add, subtract, intersect }
    struct Triangle
    {
        int a, b, c;
    }
    public static Mesh ClipMeshes(Mesh A, Mesh B, MergeMode mode)
    {
        List<Vector3> verts;

        return new Mesh();

    }

    public static Vector3 Bezerp(Vector3[] control, float t)
    {
        Vector3[] nControl = new Vector3[control.Length - 1];
        for (int i = 0; i < nControl.Length; ++i)
        {
            nControl[i] = Vector3.Lerp(control[i], control[i + 1], t);
        }

        if (nControl.Length == 1)
            return nControl[0];
        else
            return Bezerp(nControl, t);
    }

    public static Vector3 Trerp(Vector3 c, Vector3 xv, Vector3 yv, float x, float y)
    {
        return (x * (xv - c) + y * (yv - c));
    }

    public static Vector3 BezerpTri(Vector3[,] control, int size, float x, float y)
    {
        if (size == 1)
            return Trerp(control[0,0], control[0,1], control[1,0], x, y);
        else
        {
            Vector3[,] ncon = new Vector3[size, size];
            for (int i = 0; i < size; ++i) for (int j = 0; j < size - i; ++j)
                    ncon[i, j] = Trerp(control[i, j], control[i, j + 1], control[i + 1, j], x, y);
            return BezerpTri(ncon, size - 1, x, y);
        }
    }

    public static Vector3 BezerpQuad(Vector3[,] control, float x, float y)
    {
        Vector3[] ycon = new Vector3[control.GetLength(0)];
        for(int i = 0; i < control.GetLength(0); ++i)
        {
            Vector3[] xcon = new Vector3[control.GetLength(1)];
            for (int j = 0; j < control.GetLength(1); ++j)
                xcon[j] = control[i, j];

            ycon[i] = Bezerp(xcon, x);
        }
        return Bezerp(ycon, y);
    }

    public static Vector3 raySphereHit(Vector3 origin, Vector3 direction, Vector3 center, float radius)
    {
        direction = direction.normalized;

        float dot = Vector3.Dot(direction, origin - center);
        float sq = dot * dot - (origin - center).sqrMagnitude + radius * radius;
        float d = -dot + Mathf.Sqrt(sq);

        return origin + direction * d;
    }

    /*public static Vector3[] ReControlBezier(Vector3[] control, int count)
    {

    }*/
}
