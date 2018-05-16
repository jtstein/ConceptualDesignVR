using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSolid : Solid
{
    public Point center;
    public Point shell;

    float radius;

    public SphereSolid(Point center, Point shell)
    {
        this.center = center;
        this.shell = shell;
        faces.Add(new SphereFace(center, shell));
        faces[0].solids.Add(this);
        radius = (center.position - shell.position).magnitude;
    }

    public override void Render(Material mat = null)
    {
        Graphics.DrawMeshNow(GeometryUtil.icoSphere3, Matrix4x4.TRS(center.position, Quaternion.identity, new Vector3(radius, radius, radius)));
    }

    public override void Update()
    {
        radius = (center.position - shell.position).magnitude;
    }
}
