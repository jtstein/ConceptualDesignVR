using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapToolButton : ToggleButton {

	Controller controller;
    public string tool;
    public System.Type toolType;
    float cdTime;
    
    new void Start()
    {
        base.Start();
        controller = GameObject.Find("LoPoly_Rigged_Hand_Right").GetComponent<handController>();
    }

    public override void ToggleOn()
    {
        Debug.Log(tool); 
        if (controller == null)
        {
            controller = GameObject.Find("LoPoly_Rigged_Hand_Right").GetComponent<handController>();
        }

        controller.changeTool(tool);
        //ToggleOffAllOtherTools();
        foreach(HUDFrame frame in HUD.frameStack)
        {
            GameObject gameObject = frame.gameObject;
            foreach (SwapToolButton b in gameObject.GetComponentsInChildren<SwapToolButton>())
                if (b.toggled && b != this)
                {
                    b.BasePress();
                }
        } 
            
    }
    public override void OnPress()
    {
        GameObject go = GameObject.Find("Tools");
        if (go.transform.Find(tool).gameObject.activeSelf)
            return;
        base.OnPress();
    }
    public void BasePress()
    {
        base.OnPress();
    }
}
