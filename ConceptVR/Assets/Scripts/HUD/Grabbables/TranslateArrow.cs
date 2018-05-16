using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateArrow : Grabbable {
    public Vector3 axis;

    Vector3 oDiff;

    new void Start()
    {
        base.Start();
        axis = axis.normalized;
        oDiff = transform.position - transform.parent.position;
    }

    public override void whileGrabbed()
    {
        Vector3 pDiff = Vector3.Project(transform.position - startGrabPos, axis);
        Vector3 pPos = startGrabPos + pDiff;
        transform.parent.position = pPos - oDiff;
        transform.position = pPos;
    }
}
