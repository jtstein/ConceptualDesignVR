using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTool : Tool {
    public Material selMat;

    ItemBase itemBase;

    protected float defaultSelectDistance = .045f;
    protected float selectDistance = 0.45f;
    public void Start()
    {
        itemBase = GameObject.Find("ItemBase").GetComponent<ItemBase>();
    }

    public override void Update()
    {
        base.Update();
        selMat.SetFloat("_Offset", Time.time / 6f);
        RenderObject();
    }

    public override bool Tap(Vector3 position)
    {
        float playerScale = GameObject.Find("Managers").GetComponent<SettingsManager>().playerScale;
        selectDistance = defaultSelectDistance * playerScale * 0.50f;
        DCGElement nearestElement = DCGBase.NearestElement(position, selectDistance);
        Item item = ItemBase.itemBase.findNearestItem(position);
        return (item == null && nearestElement == null) ? false : 
            (item == null ? TapDCG(nearestElement) : 
            (nearestElement == null ? TapItem(item) : 
            (item.Distance(position) > nearestElement.Distance(position) ? TapDCG(nearestElement) : TapItem(item))));
        
    }
    public override bool Swipe()
    {
        if (DCGBase.sElements.Count > 0)
        {
            ClearSelection();
            return true;
        }
        Deselect();
        return false;
    }

    public List<Point> Select(DCGElement elem)
    {
        elem.isSelected = true;
        elem.Lock();
        if (ItemBase.sItems.Count > 0)
            Item.Pop();
        return elem.GetPoints();
    }
    public void Select(Item item)
    {
        item.CmdSelect();
        ItemBase.sItems.Add(item);
        if(DCGBase.sPoints.Count == 0)
            itemBase.itemHudManager(item);
    }

    public void ClearSelection()
    {
        foreach(DCGElement e in DCGBase.sElements)
        {
            Deselect(e);
        }

        DCGBase.sElements = new List<DCGElement>();
        DCGBase.sPoints = new List<Point>();
    }
    public void DeselectChildren(DCGElement e)
    {
        List<DCGElement> elems = e.GetChildren();
        foreach(DCGElement elem in elems)
        {
            if (elem.isSelected)
            {
                Deselect(elem);
                DCGBase.sElements.Remove(elem);
            }
        }
    }

    public void Deselect(DCGElement e)
    {
        e.isSelected = false;
        e.Unlock();
    }
    public void Deselect()
    {
        foreach (Item item in ItemBase.sItems)
        {
            item.CmdDeSelect();
        }
        ItemBase.sItems.Clear();
        if (!Item.popped)
        {
            Item.Pop();
            itemBase.firstType = "";
        }
    }

    protected void RenderObject()
    {
        foreach (DCGElement e in DCGBase.sElements)
        {
            e.Render(selMat);
        }
    }

    void OnDisable()
    {
        System.Type type = GameObject.Find("LoPoly_Rigged_Hand_Right").gameObject.GetComponent<handController>().currentTool.GetType();
        if(type.IsSubclassOf(typeof(SelectTool)) || type == typeof(SelectTool))
            return;
        ClearSelection();
        if (ItemBase.sItems.Count != 0)
            Deselect();
    }
    public virtual bool TapDCG(DCGElement nearestElement)
    {
        if(nearestElement == null)
        {
            return false;
        }
        if (nearestElement.ParentSelected())
        {
            return false;
        }
        if (nearestElement != null && !nearestElement.isLocked)
        {
            List<Point> newSel = Select(nearestElement);
            DeselectChildren(nearestElement);
            DCGBase.sElements.Remove(nearestElement);
            DCGBase.sElements.Add(nearestElement);
            foreach (Point p in newSel)
            {
                DCGBase.sPoints.Remove(p); //If the point exists in the point list, remove the copy before adding it in
                DCGBase.sPoints.Add(p);
            }
            if(nearestElement.GetType() != typeof(Solid))
            {
                List<DCGElement> elems = nearestElement.GetParents();
                foreach(DCGElement elem in elems)
                {
                    TapDCG(elem);
                }
            }
            else
            {
                Solid s = (Solid)nearestElement;
                s.addPointsToDCG();
            }
            return true;
        }
        else
            return false;
    }
    public bool TapItem(Item item)
    {
        if (item != null && !item.isLocked)
        {
            Select(item);
            return true;
        }
        else
            return false;
    }
}
