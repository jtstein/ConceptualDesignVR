using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensitySlider : HUDSlider {

  //  GameObject selectedLight;
   // const float INTENSITY_SCALE = 10;

	// Use this for initialization
	void Start () {
        //selectedLight = GameObject.Find("SelectItemsTool").GetComponent<SelectItemsTool>().selected;
        base.Start();
    }

    // Update is called once per frame
    void Update () {

        base.Update();
	}

    public new void OnPress()
    {
        base.OnPress();
        /*
        selectedLight = GameObject.Find("SelectItemsTool").GetComponent<SelectItemsTool>().selected;

        if (selectedLight.tag == "Light")
        {
            selectedLight.GetComponent<Light>().intensity = value * INTENSITY_SCALE;
        }
        */
    }


}
