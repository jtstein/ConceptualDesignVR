using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTool : Tool {
    Vector3 grabPos;
    Vector3 startPos;
    Point grabbedPoint;
    float maxDist = 0.03f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	new void Update () {
        base.Update();
        if (triggerInput && grabbedPoint != null)
            grabbedPoint.setPositionSnap(startPos + controllerPosition - grabPos);
	}

    public override bool TriggerDown()
    {
        grabbedPoint = DCGBase.NearestPoint(controllerPosition, maxDist);
        if (grabbedPoint != null)
        {
            grabPos = controllerPosition;
            startPos = grabbedPoint.position;
            return true;
        }
        return false;
    }

    public override bool TriggerUp()
    {
        if (grabbedPoint != null)
        {
            grabbedPoint = null;
            return true;
        }
        return false;
    }

    public override bool Tap(Vector3 position)
    {
        Point p = new Point(position);

        //points on faces/ edges
        /*Edge nE = DCGBase.NearestEdge(position, maxDist);
        Face nF = DCGBase.NearestFace(position, maxDist);

        if (nE != null && nF != null)
        {
            DCGConstraint cE = nE.NearestConstraint(position);
            cE.pointID = p.elementID;
            DCGConstraint cF = nF.NearestConstraint(position);
            cF.pointID = p.elementID;
            Vector3 pE = nE.ConstraintPosition(cE.constraintData);
            Vector3 pF = nF.ConstraintPosition(cF.constraintData);

            if (Vector3.Distance(position, pE) < Vector3.Distance(position, pF))
            {
                DCGBase.instance.AddConstraint(cE);
                p.position = nE.ConstraintPosition(cE.constraintData);
            }
            else
            {
                DCGBase.instance.AddConstraint(cF);
                p.position = nF.ConstraintPosition(cF.constraintData);
            }
        }
        else if (nE != null)
        {
            DCGConstraint cE = nE.NearestConstraint(position);
            cE.pointID = p.elementID;
            DCGBase.instance.AddConstraint(cE);
            p.position = nE.ConstraintPosition(cE.constraintData);
        }
        else if (nF != null)
        {
            DCGConstraint cF = nF.NearestConstraint(position);
            cF.pointID = p.elementID;
            DCGBase.instance.AddConstraint(cF);
            p.position = nF.ConstraintPosition(cF.constraintData);
        }*/

        return true;
    }
}
