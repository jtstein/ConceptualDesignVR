using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkTool : SelectTool {
    public Material linkMat;

	// Use this for initialization
	new void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	new void Update () {
        RenderObject();
	}

    public override bool TriggerDown()
    {
        if (DCGBase.sPoints.Count <= 1) return false;
        if (DCGBase.sPoints.Count == 2)
        {
            new Edge(DCGBase.sPoints[0], DCGBase.sPoints[1]);
        }
        else if (DCGBase.sPoints.Count >= 3)
        {
            List<Edge> newEdges = new List<Edge>();
            for(int i = 0; i < DCGBase.sPoints.Count; ++i)
            {
                Edge newEdge = null;

                foreach (Edge e in DCGBase.sPoints[i].edges) //check if this edge already exists
                    if (e.HasEndpoints(DCGBase.sPoints[i], DCGBase.sPoints[(i + 1) % DCGBase.sPoints.Count]))
                        newEdge = e;

                if (newEdge == null)    //create this edge if it doesn't already exist
                    newEdge = new Edge(DCGBase.sPoints[i], DCGBase.sPoints[(i + 1) % DCGBase.sPoints.Count]);

                newEdges.Add(newEdge);
            }
            new Face(newEdges);
        }

        Solid nSolid = Solid.FindClosedSurface(DCGBase.sPoints[0]);
        if (nSolid != null)
            Debug.Log("Completed a shapesome thing!");

        ClearSelection();
        return true;
    }

    new void RenderObject()
    {
        base.RenderObject();

        //linkMat.SetPass(0);

        Vector3 edgeVec;
        if (DCGBase.sPoints.Count > 2)
            for (int i = 0; i < DCGBase.sPoints.Count - 1; ++i)
            {
                edgeVec = DCGBase.sPoints[i].position - DCGBase.sPoints[i + 1].position;
                Graphics.DrawMesh(GeometryUtil.cylinder8, Matrix4x4.TRS(DCGBase.sPoints[i].position - edgeVec / 2, Quaternion.FromToRotation(Vector3.up, edgeVec), new Vector3(.005f, edgeVec.magnitude / 2, .005f)),linkMat,0);
            }
        if (DCGBase.sPoints.Count > 1)
        {
            edgeVec = DCGBase.sPoints[DCGBase.sPoints.Count - 1].position - DCGBase.sPoints[0].position;
            Graphics.DrawMesh(GeometryUtil.cylinder8, Matrix4x4.TRS(DCGBase.sPoints[0].position + edgeVec / 2, Quaternion.FromToRotation(Vector3.up, edgeVec), new Vector3(.005f, edgeVec.magnitude / 2, .005f)), linkMat, 0);
            //Graphics.DrawMeshNow(GeometryUtil.cylinder8, Matrix4x4.TRS(DCGBase.sPoints[0].position + edgeVec / 2, Quaternion.FromToRotation(Vector3.up, edgeVec), new Vector3(.005f, edgeVec.magnitude / 2, .005f)));
        }
    }
}
