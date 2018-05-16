using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTool : SelectTool {
    List<Vector3> startPositions;   //positions of the selected points at start
    List<Vector3> startPos;
    Vector3 grabPosition;   //position of the tool at start
    Quaternion grabOrientation; //rotation of the tool at start

    // Update is called once per frame
    new void Update () {
        base.Update();

        if (triggerInput && DCGBase.sPoints != null)
        {
            for (int i = 0; i < DCGBase.sPoints.Count; ++i)
            {
                DCGBase.sPoints[i].setPosition(startPositions[i] + controllerPosition - grabPosition);
            }
        }
        if (triggerInput && ItemBase.sItems != null)
            for (int i = 0; i < ItemBase.sItems.Count; ++i)
                ItemBase.sItems[i].changePosition(startPos[i], controllerPosition, grabPosition);
    }

    public override bool TriggerDown()
    {
        if (DCGBase.sPoints.Count == 0 && ItemBase.sItems.Count == 0)
            return false;
        if (DCGBase.sPoints.Count > 0)
        {
            startPositions = new List<Vector3>(DCGBase.sPoints.Count);
            Debug.Log(DCGBase.sPoints.Count);
            foreach (Point p in DCGBase.sPoints)
                startPositions.Add(p.position);
        }
        if(ItemBase.sItems.Count > 0)
        {
            startPos = new List<Vector3>();
            foreach (Item v in ItemBase.sItems)
                startPos.Add(v.Position());
        }

        grabPosition = controllerPosition;
        return true;
    }
    public override bool DualTriggerDown()
    {
        if (DCGBase.sElements.Count == 0)
            return true;
        List<DCGElement> copiedElements = new List<DCGElement>();
        foreach (DCGElement d in DCGBase.sElements)
        {
            copiedElements.Add(d.Copy(DCGBase.nextMoveID()));
        }

        return true;
    }

    public override bool TriggerUp()
    {
        if (startPositions != null)
        {
            startPositions = null;
            return true;
        }
        else
            return false;
    }
}
