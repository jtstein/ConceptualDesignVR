using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTool : Tool {
    Point currentCenter;
    Point currentShell;

    void Update()
    {
        if (triggerInput)
        {
            currentShell.setPosition(controllerPosition);
        }
    }

    public override bool TriggerDown()
    {
        currentCenter = new Point(controllerPosition);
        currentShell = new Point(controllerPosition);
        new SphereSolid(currentCenter, currentShell);
        return true;
    }

    public override bool TriggerUp()
    {
        currentCenter = null;
        currentShell = null;
        return true;
    }
}
