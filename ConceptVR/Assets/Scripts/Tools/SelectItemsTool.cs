using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum ListContains { lights, doodles } // this will be expanded as we gain more items, lol nvm, not using this, but keeping for laughs
public class SelectItemsTool : Tool {

    ItemBase itemBase;
    Vector3 holdPos;
    List<Vector3> startPos;

    // Use this for initialization
    void Start () {
        itemBase = GameObject.Find("ItemBase").GetComponent<ItemBase>();
	}
	
	// Update is called once per frame
	void Update () {
        if (triggerInput && ItemBase.sItems != null)
            for (int i = 0; i < ItemBase.sItems.Count; ++i)
                ItemBase.sItems[i].changePosition(startPos[i], controllerPosition, holdPos);
    }

    public override bool Swipe()
    {
        if (ItemBase.sItems != null)
        {
            Deselect();
            return true;
        }
        return false;
    }
    public override bool Tap(Vector3 position)
    {
        Item item = itemBase.findNearestItem(position);
        if(item != null && !item.isLocked)
        {
            Select(item);
            return true;
        }
        else
            return false;
    }
    public override bool TriggerDown()
    {
        if (ItemBase.sItems.Count == 0)
            return false;
        startPos = new List<Vector3>();
        foreach (Item v in ItemBase.sItems)
            startPos.Add(v.Position());
        holdPos = controllerPosition;
        return true;
    }

    public void Deselect()
    {
        foreach(Item item in ItemBase.sItems)
        {
            item.CmdDeSelect();
        }
        ItemBase.sItems.Clear();
        if (itemBase.isHUD)
        {
            Item.Pop();
            itemBase.firstType = "";
        }
    }
    public void Select(Item item)
    {
        item.CmdSelect();
        ItemBase.sItems.Add(item);
        itemBase.itemHudManager(item);
    }
    private void OnDisable()
    {
        if(ItemBase.sItems.Count != 0)
            Deselect();
    }


}
