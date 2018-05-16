using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDTrigger : MonoBehaviour {
    HUDButton parent;

	// Use this for initialization
	void Start () {
        parent = GetComponentInParent<HUDButton>();
	}

    public void OnTriggerEnter(Collider other)
    {
        parent.OnSubTriggerEnter(other, this.name);
    }

    public void OnTriggerExit(Collider other)
    {
        parent.OnSubTriggerExit(other, this.name);
    }
}
