using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFrame : HUDFrame {

    HUDSlider intensitySlider;
    HUDSlider colorSlider;

	// Use this for initialization
	void Start () {
        intensitySlider = this.gameObject.transform.Find("IntensitySlider").GetComponent<HUDSlider>();
        colorSlider = this.gameObject.transform.Find("ColorSlider").GetComponent<HUDSlider>();
        UnityEngine.Rendering.GraphicsSettings.lightsUseColorTemperature = true;
        UnityEngine.Rendering.GraphicsSettings.lightsUseLinearIntensity = true;
    }

    // Update is called once per frame
    void Update () {
        foreach (LightItem light in ItemBase.sItems)
        {
            Color lightColor = kelvinToRGB((1-colorSlider.value) * 40000f);
            light.changeColor(lightColor);
            light.changeIntensity(intensitySlider.value);
        }
    }

    Color kelvinToRGB(float k)
    {
        if (k < 1000)
        {
            k = 1000;
        }
        if (k > 40000)
        {
            k = 40000;
        }
        k /= 200;

        float temp = 0;
        float r = 0f;
        float b = 0f;
        float g = 0f;

        if (k <= 66)
        {
            r = 255;
        }
        else
        {
            temp = k - 60;
            temp = 329.698727446f * Mathf.Pow(temp, -0.1332047592f);
            r = temp;
            if (r < 0)
            {
                r = 0;
            }
            if (r > 255)
            {
                r = 255;
            }
        }

        if (k <= 66)
        {
            temp = k;
            temp = 99.4708025861f * Mathf.Log(temp) - 161.1195681661f;
            g = temp;
            if (g < 0)
            {
                g = 0;
            }
            if (g > 255)
            {
                g = 255;
            }
        }
        else
        {
            temp = k - 60;
            temp = 288.1221695283f * (Mathf.Pow(temp, -0.0755148492f));
            g = temp;
            if (g < 0)
            {
                g = 0;
            }
            if (g > 255)
            {
                g = 255;
            }
        }

        if (k >= 66)
        {
            b = 255;
        }
        else if (k <= 19)
        {
            b = 0;
        }
        else
        {

            temp = k - 10;
            temp = 138.5177312231f * Mathf.Log(temp) - 305.0447927307f;
            b = temp;
            if (b < 0)
            {
                b = 0;
            }
            if (b > 255)
            {
                b = 255;
            }
        }

        return new Color(r/255,g/255,b/255,1);
    }

}
