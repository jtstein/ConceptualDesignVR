using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereFace : Face {
    public Point center;
    public Point shell;

    float radius;

    public SphereFace(Point center, Point shell)
    {
        this.center = center;
        this.shell = shell;
        radius = (center.position - shell.position).magnitude;
        center.elements.Add(this);
        shell.elements.Add(this);
    }
    
    public override void Render(Material mat = null) {
        Graphics.DrawMeshNow(GeometryUtil.icoSphere3, Matrix4x4.TRS(center.position, Quaternion.identity, new Vector3(radius, radius, radius)));
    }

    public override void Update()
    {
        radius = (center.position - shell.position).magnitude;

        foreach (Solid s in solids) if (s.lastMoveID != lastMoveID)
            {
                s.lastMoveID = lastMoveID;
                s.Update();
            }
    }
}
