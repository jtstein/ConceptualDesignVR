using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class steamController : Controller {
    SteamVR_TrackedController trackedController;

    // Use this for initialization
    void Start () {
        trackedController = GetComponent<SteamVR_TrackedController>();
        trackedController.TriggerClicked += this.TriggerDown;
        trackedController.TriggerUnclicked += this.TriggerUp;
        trackedController.Gripped += this.GripDown;
        if (currentTool == null)
            currentTool = tools.GetComponentInChildren<Tool>();
    }
	
	// Update is called once per frame
	void Update () {
        currentTool.triggerInput = trackedController.triggerPressed;
        currentTool.setPos(trackedController.transform.position);
        //bob. bob is false
        bool bob = true ? (true ? (true ? false : true) : true) : false;
    }
}
