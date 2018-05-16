using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap;

public class handController: Controller {
    LeapTrackedController leapControl;
    private Tool lastTool;
    // Use this for initialization
    new void Start () {
        base.Start();
        leapControl = GetComponent<LeapTrackedController>();
        leapControl.pinchMade += TriggerDown;
        leapControl.pinchGone += TriggerUp;
        leapControl.dualPinchMade += DualTriggerUp;
        leapControl.dualPinchGone += DualTriggerDown;
        leapControl.grabMade += GripDown;
        leapControl.grabGone += GripUp;
        leapControl.tapMade += Tap;
        leapControl.swipeMade += Swipe;
        leapControl.freeForm += freeForm;
        leapControl.freeFormEnd += freeFormEnd;
        leapControl.fireGun += fireGun;
    }

    // Update is called once per frame
    void Update ()
    {
        foreach (Tool tool in ToolQueue)
        {
            tool.setPos(leapControl.position);
            currentTool.triggerInput = leapControl.pinchHeld;
        }

        if (currentTool != null)
        {
            currentTool.setPos(leapControl.position);

            //Check to see if a pinch is being held.
            if (currentTool.GetType() == typeof(FreeFormTool))
            {

                if (leapControl.forming)
                    currentTool.formInput = true;
                else
                    currentTool.formInput = false;
            }
            else{
               
                currentTool.triggerInput = leapControl.pinchHeld;
            }

        }

    }

    //Pinch down
    protected void TriggerDown()
    {
        if (currentTool != null && currentTool.TriggerDown())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.TriggerDown())
                return;
    }
    //pinch up
    protected void TriggerUp()
    {
        if (currentTool != null && currentTool.TriggerUp())
            return;
        else
        foreach (Tool tool in ToolQueue)
            if (tool.TriggerUp())
                return;
    }
    //left pinch down
    protected void DualTriggerDown()
    {
        if (currentTool != null && currentTool.DualTriggerDown())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.DualTriggerDown())
                return;
    }
    //left [pinch up
    protected void DualTriggerUp()
    {
        if (currentTool != null && currentTool.DualTriggerUp())
            return;
        else
            foreach (Tool tool in ToolQueue)
                if (tool.DualTriggerUp())
                    return;
    }
    //Deprecated
    protected void GripUp()
    {
        if (currentTool.GripUp())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.GripUp())
                return;
    }
    //Deprecated
    protected void GripDown()
    {
        if (currentTool.GripDown())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.GripDown())
                return;
    }
    //Tap event handler
    protected void Tap(Vector3 position)
    {
        if (currentTool.Tap(position))
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.Tap(position))
                return;
    }
    //swipe event handler
    protected void Swipe()
    {
        if (currentTool.Swipe())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.Swipe())
                return;
    }
    //freeform start event handler
    protected void freeForm()
    {
        lastTool = currentTool;
        Debug.Log(currentTool);
        changeTool("FreeFormTool");
        currentTool.FreeForm(leapControl);
    }
    //free form event stop
    protected void freeFormEnd()
    {
        currentTool.FreeFormEnd();
        changeTool(lastTool.GetType().ToString());
    }
    //free form failure event handler
    protected void freeFormFailure()
    {
        currentTool.formInput = false;
        changeTool(lastTool.GetType().ToString());
    }
    //gun fire event handler
    protected void fireGun()
    {
        if (currentTool.Fire())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.Fire())
                return;
    }
    void OnEnable(){
        
    }

}
