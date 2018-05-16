using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTutorial : ToggleButton {

	// Use this for initialization
	void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}

    public override void OnPress()
    {
        GameObject.Find("Managers").GetComponent<SettingsManager>().toggleTutorialMode();
        base.OnPress();
    }
}
