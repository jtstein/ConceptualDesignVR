using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExtrudeTool : MoveTool {

    List<List<DCGElement>> extrudedUndo;
    int count = 0;
    public void Start()
    {
        extrudedUndo = new List<List<DCGElement>>();
        base.Start();
    }
    public override bool TriggerDown()
    {
        Debug.Log(DCGBase.sElements.Count);
        if (DCGBase.sElements == null || DCGBase.sElements.Count == 0)
            return false;
        //This is the undo section and it is disgusting. Please ask me to die later. k thx
        List<DCGElement> newSElement = new List<DCGElement>();
        foreach(DCGElement e in DCGBase.sElements){
            if (e.GetType() == typeof(Solid)) continue;
            List<DCGElement> temp = e.Extrude();
            newSElement.Add(temp[temp.Count-1]);
            extrudedUndo.Add(temp);
        }

        ClearSelection();
        foreach (DCGElement elem in newSElement)
            TapDCG(elem);//It's ghetto dont care.
        base.TriggerDown();
        return true;
    }
    public override bool TriggerUp()
    {
        if(DCGBase.sElements.Count != 0)
            ClearSelection();
        return true;
    }
    public override bool Swipe()
    {
        if (extrudedUndo.Count == 0)
            return true;
        List<DCGElement> lister = extrudedUndo[extrudedUndo.Count - 1];
        lister[0].Copy(DCGBase.nextMoveID());
        removeElems(lister);
        extrudedUndo.RemoveAt(extrudedUndo.Count - 1);
        return true;
    }
    public void removeElems(List<DCGElement> lister)
    {
        foreach (DCGElement elem in lister)
        {
            //If anyone reads this.... I am sorry, this was the only way to fix this, blame C#'s memory clean up.
            try
            {
                elem.Remove();
            }
            catch (System.Exception e)
            {
                //I deem this recursive failure of succesful correct
                removeElems(lister);
                return;
            }
        }
        lister.Clear();
    }
    public override bool TapDCG(DCGElement nearestElement)
    {
        if (nearestElement != null && !nearestElement.isLocked)
        {
            List<Point> newSel = Select(nearestElement);
            DCGBase.sElements.Remove(nearestElement);
            DCGBase.sElements.Add(nearestElement);
            foreach (Point p in newSel)
            {
                DCGBase.sPoints.Remove(p); //If the point exists in the point list, remove the copy before adding it in
                DCGBase.sPoints.Add(p);
            }
            return true;
        }
        else
            return false;
    }
    public void OnDisable()
    {
        System.Type type = GameObject.Find("LoPoly_Rigged_Hand_Right").gameObject.GetComponent<handController>().currentTool.GetType();
        if (type.IsSubclassOf(typeof(SelectTool)) || type == typeof(SelectTool))
            return;
        ClearSelection();
        if (ItemBase.sItems.Count != 0)
            Deselect();
        extrudedUndo = new List<List<DCGElement>>();
    }
}
