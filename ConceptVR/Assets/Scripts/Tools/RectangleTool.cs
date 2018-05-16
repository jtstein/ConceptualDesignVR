using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RectangleTool : Tool {

    Vector3 startPosition;
    List<Vector3> verts;
    float time;
    GameObject ghost;

    private void Start()
    {
        ghost = Instantiate(this.gameObject.transform.Find("RectangleGhost").gameObject);
        ghost.gameObject.SetActive(true);
        ghost.transform.position = controllerPosition;
        ghost.gameObject.SetActive(false);
        time = Time.time;
        startPosition = new Vector3(0f, 0f, 0f);
    }

    public override void Update()
    {
        if (triggerInput)
        {
            UpdateVerts();
        }
    }

    public override bool TriggerDown()
    {
        ghost.transform.localScale = Vector3.zero;
        startPosition = controllerPosition;
        ghost.gameObject.SetActive(true);
        ghost.transform.position = startPosition;
        return true;
    }

    public override bool TriggerUp()
    {
        // trigger up gets called twice >> BUG 
        // this cooldown hack sets it so GenerateRectangle() only gets called every half a second
        if (time + 0.5f < Time.time)
        {
            GenerateRectangle();
            time = Time.time;
        }
        verts.Clear();
        ghost.gameObject.SetActive(false);
        return true;
    }

    private void GenerateRectangle()
    {
        List<Face> faces = new List<Face>();
        List<Edge> edges = new List<Edge>();
        Point p1 = new Point(verts[0]);
        Point p2 = new Point(verts[1]);
        Point p3 = new Point(verts[2]);
        Point p4 = new Point(verts[3]);
        Point p5 = new Point(verts[4]);
        Point p6 = new Point(verts[5]);
        Point p7 = new Point(verts[6]);
        Point p8 = new Point(verts[7]);

        // front face
        edges.Add(new Edge(p1, p2));
        edges.Add(new Edge(p2, p4));
        edges.Add(new Edge(p4, p3));
        edges.Add(new Edge(p3, p1));
        faces.Add(new Face(edges));
        edges.Clear();

        // back face
        edges.Add(new Edge(p5, p6));
        edges.Add(new Edge(p6, p8));
        edges.Add(new Edge(p8, p7));
        edges.Add(new Edge(p7, p5));
        faces.Add(new Face(edges));
        edges.Clear();

        // left face
        edges.Add(new Edge(p1, p5));
        edges.Add(new Edge(p5, p7));
        edges.Add(new Edge(p3, p7));
        edges.Add(new Edge(p3, p1));
        faces.Add(new Face(edges));
        edges.Clear();

        // right face
        edges.Add(new Edge(p2, p6));
        edges.Add(new Edge(p6, p8));
        edges.Add(new Edge(p8, p4));
        edges.Add(new Edge(p4, p2));
        faces.Add(new Face(edges));
        edges.Clear();

        // top face
        edges.Add(new Edge(p1, p5));
        edges.Add(new Edge(p5, p6));
        edges.Add(new Edge(p6, p2));
        edges.Add(new Edge(p2, p1));
        faces.Add(new Face(edges));
        edges.Clear();

        // bottom face
        edges.Add(new Edge(p3, p7));
        edges.Add(new Edge(p7, p8));
        edges.Add(new Edge(p8, p4));
        edges.Add(new Edge(p4, p3));
        faces.Add(new Face(edges));
        edges.Clear();

        //Solid rectangle = new Solid(faces);
    }

    private void UpdateVerts()
    {
        verts = new List<Vector3>();
        Vector3 endPosition = controllerPosition;

        verts.Add(startPosition);
        verts.Add(new Vector3(endPosition.x, startPosition.y, startPosition.z));
        verts.Add(new Vector3(startPosition.x, endPosition.y, startPosition.z));
        verts.Add(new Vector3(endPosition.x, endPosition.y, startPosition.z));
        verts.Add(new Vector3(startPosition.x, startPosition.y, endPosition.z));
        verts.Add(new Vector3(endPosition.x, startPosition.y, endPosition.z));
        verts.Add(new Vector3(startPosition.x, endPosition.y, endPosition.z));
        verts.Add(endPosition);

        // update scale of ghost
        ghost.transform.localScale = startPosition - controllerPosition;
        // update position of ghost to be half of the scale so it is anchored to the start position
        ghost.transform.position = startPosition - (startPosition - controllerPosition) / 2;
    } 
}
