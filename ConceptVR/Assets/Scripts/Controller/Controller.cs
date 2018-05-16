using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public abstract class Controller : MonoBehaviour {
    public bool hand;//Right is true, left is false;
    public Controller other;    //controller attached to the other hand (it is assumed the user only has two hands)
    public GameObject tools;
    public GameObject videos;
    SettingsManager sm;
    public List<Tool> ToolQueue;
    public Tool currentTool;
    public VideoClip currentVideo;

	// Use this for initialization
	protected void Start () {
        sm = GameObject.Find("Managers").GetComponent<SettingsManager>();
        if (ToolQueue.Count == 0)
        {
            ToolQueue = new List<Tool>();
            BaseTool bt = GameObject.Find("BaseTool").GetComponent<BaseTool>();
            ToolQueue.Add(bt);
        }
        currentTool = ToolQueue[0];
        currentVideo = null;
        tools = this.gameObject.transform.parent.gameObject.transform.Find("Tools").gameObject;
    }
	
	// Update is called once per frame
	void Update () {

    }
    //Event handlers for activiating tools
    //Pinch down
    protected void TriggerDown(object sender, ClickedEventArgs e)
    {
        if (currentTool.TriggerDown())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.TriggerDown())
                return;
    }
    //Pinch up
    protected void TriggerUp(object sender, ClickedEventArgs e)
    {
        currentTool.triggerInput = false;
        if (currentTool.TriggerUp())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.TriggerUp())
                return;
    }
    protected void LaserPointer()
    {

    }
    //Deprecated, grab down
    protected void GripDown(object sender, ClickedEventArgs e)
    {
        if (currentTool.GripDown())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.GripDown())
                return;
    }
    //Deprecated, grab up
    protected void GripUp(object sender, ClickedEventArgs e)
    {
        if (currentTool.GripUp())
            return;
        foreach (Tool tool in ToolQueue)
            if (tool.GripUp())
                return;
    }
    /*
     * changeTool
     * Input - String: name of the tool
     * Output: None- current tool gets changed into the input tool
     * */
    public void changeTool(string toolName)
    {
        Tool lastTool = currentTool;
        currentTool = tools.transform.Find(toolName).GetComponent<Tool>();
        System.Type curType = currentTool.GetType();
        System.Type lastType = null;
        if (lastTool.GetType() != null)
        {
            lastType = lastTool.GetType();
        }
        lastType = lastTool.GetType();
        //Debug.Log(toolName);
        
        if ((curType != lastType) || (curType == typeof(SpecificSelectTool) && lastType == typeof(SpecificSelectTool)))
        {
            deactivateLastTool(lastTool);
            activateNewTool(currentTool);
        }

        //Debug.Log(currentTool.GetType());
    }
    /*
     * GetToolByName
     * Input - String:name of tool
     * Output: Tool: Tghe tool class of the chosen tool.
     */
    public Tool GetToolByName(String name)
    {
        return tools.transform.Find(name).GetComponent<Tool>();
    }
    //Tool activation, activates game objects of the tool
    public static void deactivateLastTool(Tool t) {
        if (t)
            t.gameObject.SetActive(false);
    }
    public void activateNewTool(Tool t)
    {
        currentVideo = t.videoClip;
        if (currentVideo != null && sm.tutorialMode)
        {
            sm.updateTutorialVideoClip(currentVideo);
        }
        t.gameObject.SetActive(true);
    }
    public void OnEnable()
    {
        if (currentTool == null) currentTool = tools.GetComponentInChildren<Tool>();
        foreach (Tool t in tools.GetComponentsInChildren<Tool>())
            deactivateLastTool(t);
    }
}
