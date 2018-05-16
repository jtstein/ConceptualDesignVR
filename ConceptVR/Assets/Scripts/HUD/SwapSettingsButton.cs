using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapSettingsButton : ToggleButton
{

    Controller controller;
    public string setting;
    public System.Type toolType;
    float cdTime;

    new void Start()
    {
        base.Start();
        controller = GameObject.Find("LoPoly_Rigged_Hand_Right").GetComponent<handController>();
    }

    public override void ToggleOn()
    {
        foreach (SwapSettingsButton b in transform.parent.GetComponentsInChildren<SwapSettingsButton>())
            if (b.toggled && b != this)
            {
                b.OnPress();
            }
    }
    public override void OnPress()
    {
        GameObject go = GameObject.Find("Settings");
        Debug.Log("Setting " + go.transform.Find(setting).gameObject.activeSelf);
        if (go.transform.Find(setting).gameObject.activeSelf)
            return;
        base.OnPress();
    }
}
