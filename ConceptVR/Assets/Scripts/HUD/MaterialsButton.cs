using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsButton : HUDButton {

    Controller controller;

    HUDManager HUD;
    public HUDFrame materialsFrame;
    public GameObject ImportListButton;

    // prefab is the new spawned prefab.
    DCGBase dcg;
    // right index finger for tracking position of spawned prefab
    public GameObject rightIndex;

    HUDManager HUDMgr;
    HandsUtil hUtil;

    Leap.Controller leapcontroller;
    Leap.Frame frame;
    Leap.Hand lHand;

    public GameObject LeapHandController;

    // Use this for initialization
    void Start()
    {
        HUD = GameObject.Find("Managers").GetComponent<HUDManager>();
        controller = GameObject.Find("LoPoly_Rigged_Hand_Right").GetComponent<handController>();
        hUtil = new HandsUtil();
        leapcontroller = new Leap.Controller();
        frame = leapcontroller.Frame();
        HUDMgr = GameObject.Find("Managers").GetComponent<HUDManager>();
        dcg = GameObject.Find("DCG").GetComponent<DCGBase>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        
    }
    /*
    public override void ToggleOn()
    {
        GameObject go = GameObject.Find("Tools");
        Debug.Log("TOOL " + go.transform.Find("MaterialTool").gameObject.activeSelf);
        controller.changeTool("MaterialTool");
        foreach (SwapToolButton b in transform.parent.GetComponentsInChildren<SwapToolButton>())
            if (b.toggled && b != this)
            {
                b.OnPress();
            }

        HUD.Push(materialsFrame);
        base.ToggleOn();
    }

    public override void ToggleOff()
    {
        base.ToggleOff();
    }
    */

    public override void OnPress()
    {
        
        GameObject go = GameObject.Find("Tools");
        Debug.Log("TOOL " + go.transform.Find("MaterialTool").gameObject.activeSelf);
        controller.changeTool("MaterialTool");
        foreach (SwapToolButton b in transform.parent.GetComponentsInChildren<SwapToolButton>())
            if (b.toggled && b != this)
            {
                b.OnPress();
            }
            
        HUD.Push(materialsFrame);
        base.OnPress();
    }
}
