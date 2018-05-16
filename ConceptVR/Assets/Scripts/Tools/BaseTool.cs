using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTool : Tool {
    Grabbable grabbed;
    public Material tapMat;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool Tap(Vector3 position)
    {
        GameObject tapObject = new GameObject();
        tapObject.transform.position = position;
        TapRenderer tap = tapObject.AddComponent<TapRenderer>();
        tap.mat = tapMat;
        return true;
    }

    public override bool TriggerDown()
    {
        Grabbable ng = null;
        float ndist = Mathf.Infinity;
        foreach (Grabbable g in Grabbable.all)
        {
            float dist = Vector3.Distance(g.transform.position, controllerPosition);
            if (dist < g.grabRadius)
            {
                ndist = dist;
                ng = g;
            }
        }

        if (ng != null)
        {
            ng.Grab(this);
            grabbed = ng;

            return true;
        }
        else
            return false;
    }

    public override bool TriggerUp()
    {
        if (grabbed != null)
        {
            grabbed.Release();
            grabbed = null;
            return true;
        }
        else
            return false;
    }
}
