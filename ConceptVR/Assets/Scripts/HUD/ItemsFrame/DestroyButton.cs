using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyButton : HUDButton {

    ItemBase itemBase;
    SelectItemsTool selectTool;
	// Use this for initialization
	void Start () {
        itemBase = GameObject.Find("ItemBase").GetComponent<ItemBase>();
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}

    public override void OnPress()
    {
        foreach (Item item in ItemBase.sItems)
        {
            item.destroyed = true;
            ItemBase.itemBase.Remove(item);
        }
        ItemBase.sItems.Clear();
        Item.Pop();
        itemBase.firstType = "";
        base.OnPress();
    }

}
