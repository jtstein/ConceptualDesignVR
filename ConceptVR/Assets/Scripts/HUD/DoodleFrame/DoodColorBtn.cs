using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodColorBtn : HUDButton {

    public int index;
    ItemBase itembase;
    public bool newDood = true; 
	// Use this for initialization
	void Start () {
        base.Start();
        itembase = GameObject.Find("ItemBase").GetComponent<ItemBase>();
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}
    public override void OnPress()
    {
        ItemBase.changeIndex(index);
        if (ItemBase.sItems.Count != 0)
            foreach (Doodle d in ItemBase.sItems)
                d.CmdChangeColor(index);

    }
}
