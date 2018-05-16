using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensityButton : ToggleButton {

    Transform intensitySlider;

	// Use this for initialization
	void Start () {
        intensitySlider = transform.parent.gameObject.transform.Find("IntensitySlider");
        // move intensity slider slightly to right
        intensitySlider.gameObject.GetComponent<HUDButton>().endPos = new Vector3(0.175f, 0.0f, 0.0f);
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}


    public override void ToggleOn()
    {
        intensitySlider.gameObject.SetActive(true);

        base.ToggleOn();
    }

    public override void ToggleOff()
    {

        intensitySlider.gameObject.SetActive(false);
        base.ToggleOff();
    }

    public override void OnPress()
    {
        if (this.toggled)
        {
            intensitySlider.gameObject.SetActive(false);
        }
        else
        {
            intensitySlider.gameObject.SetActive(true);
        }
        base.OnPress();
    }
}
