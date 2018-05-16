using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialButton : HUDButton {

    public DCGMaterial DCGMat;
    public Tool materialTool;
    
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
        materialTool.GetComponent<MaterialTool>().changeMaterial(DCGMat);
        base.OnPress();
    }
}
