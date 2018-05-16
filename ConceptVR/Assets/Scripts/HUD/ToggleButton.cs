using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : HUDButton {
    public bool toggled;

    new void Start()
    {
        base.Start();
        
    }

    public override void OnPress()
    {
        if (toggled)
        {
            toggled = false;
            transform.Find("ViewOn").gameObject.SetActive(false);
            transform.Find("ViewOff").gameObject.SetActive(true);
            ToggleOff();
        } else
        {
            toggled = true;
            transform.Find("ViewOff").gameObject.SetActive(false);
            transform.Find("ViewOn").gameObject.SetActive(true);
            ToggleOn();
        }
    }
    new public void OnEnable()
    {
        if (!toggled)
        {
            transform.Find("ViewOn").gameObject.SetActive(false);
            transform.Find("ViewOff").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("ViewOff").gameObject.SetActive(false);
            transform.Find("ViewOn").gameObject.SetActive(true);
        }
    }

    public virtual void ToggleOn() { }
    public virtual void ToggleOff() { }
}
