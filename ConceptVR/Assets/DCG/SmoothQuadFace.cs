using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothQuadFace : Face
{
    public int resolution = 20;

    Point[,] control;

    public SmoothQuadFace(Point[,] control)
    {
        this.control = control;
    }

    public SmoothQuadFace(List<Edge> edges)
    {
        solids = new List<Solid>();
        this.edges = edges;
        foreach (Edge e in edges)
            e.faces.Add(this);

        elementID = nextElementID();
        DCGBase.faces.Add(this);
        DCGBase.all.Add(elementID, this as DCGElement);

        GenerateControl(edges);
        updateMesh();

        if (NetPlayer.local != null)
        {
            int[] pointIDs = new int[edges.Count];
            for (int i = 0; i < edges.Count; ++i)
                pointIDs[i] = edges[i].elementID;
            DCGBase.synch.CmdAddElement(elementID, pointIDs, ElementType.smoothQuad, NetPlayer.local.playerID);
        }
    }

    void GenerateControl(List<Edge> edges)
    {
        if (edges.Count != 4)
        {
            Debug.LogError("Attempted to create smooth quad with" + edges.Count + edges);
            return;
        }

        int xl = edges[0].points.Count;
        int yl = edges[1].points.Count;
        control = new Point[xl,yl];

        for (int x = 0; x < xl; ++x) control[x, 0] = edges[0].points[x];    //bottom edge
        for (int y = 0; y < yl; ++y) control[xl-1, y] = edges[1].points[y]; //right edge
        for (int x = 0; x < xl; ++x) control[x,yl-1] = edges[2].points[xl-x-1]; //top edge
        for (int y = 0; y < yl; ++y) control[0,y] = edges[3].points[yl-y-1];   //right edge

        for (int x = 1; x < xl-1; ++x) for (int y = 1; y < yl-1; ++y)
            {
                Vector3 bottom = control[x, 0].position;
                Vector3 top = control[x, yl - 1].position;
                Vector3 left = control[0, y].position;
                Vector3 right = control[yl - 1, x].position;

                Vector3 aVec = bottom + Vector3.Project((left + right) / 2 - bottom, top - bottom);
                Vector3 bVec = bottom + Vector3.Project((bottom + top) / 2 - left, right - left);
                control[x, y] = new Point((aVec + bVec) / 2);
            }
    }

    public override void updateMesh()
    {
        mesh = new Mesh();
        subTriangles = new List<Vector3>();
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> tris = new List<int>();

        Vector3[,] controlVec = new Vector3[control.GetLength(0), control.GetLength(1)];

        for (int x = 0; x < control.GetLength(0); ++x) for (int y = 0; y < control.GetLength(1); ++y)
                controlVec[x, y] = control[x, y].position;

        //generate vertices
        for (int x = 0; x <= resolution; ++x) for (int y = 0; y <= resolution; ++y)
                verts.Add(GeometryUtil.BezerpQuad(controlVec, (float)x / (resolution), (float)y / (resolution)));
        
        //generate triangles
        for (int x = 0; x < resolution; ++x) for (int y = 0; y < resolution; ++y)
            {
                int c0 = x + y * (resolution+1);
                int c1 = c0 + 1;
                int c2 = c1 + resolution;
                int c3 = c2 + 1;

                tris.Add(c0); tris.Add(c1); tris.Add(c2);
                tris.Add(c3); tris.Add(c2); tris.Add(c1);
            }

        int vertCount = verts.Count;
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        normals = new List<Vector3>(mesh.normals);

        //generate backface
        for (int i = 0; i < 6 * resolution * resolution; i += 3)
        {
            tris.Add(tris[i] + vertCount);
            tris.Add(tris[i+2] + vertCount);
            tris.Add(tris[i+1] + vertCount);
            verts.Add(verts[i/3]);
            normals.Add(-normals[i/3]);
        }


        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetNormals(normals);
        mesh.RecalculateBounds();
    }
    
    public override List<Point> GetPoints()
    {
        List<Point> points = new List<Point>();
        foreach (Point p in control)
                points.Add(p);
        return points;
    }

    public override List<DCGElement> Extrude()
    {
        Point[,] newControl = new Point[control.GetLength(0), control.GetLength(1)];
        for (int x = 0; x < control.GetLength(0); ++x) for (int y = 0; y < control.GetLength(1); ++y)
                newControl[x, y] = new Point(control[x, y].position);

        List<DCGElement> elems = new List<DCGElement>();
        elems.Add(new SmoothQuadFace(newControl));
        return elems;
    }
}
