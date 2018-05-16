using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum types { Point, Edge, Face};
public class SpecificSelectTool : SelectSphereTool {
    public types type;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (triggerInput)
        {
            DCGElement nElement;
            while (true)
            {
                float playerScale = GameObject.Find("Managers").GetComponent<SettingsManager>().playerScale;
                float selectDist = .05f * playerScale;
                nElement = selectElement(controllerPosition, selectDist);
                if (nElement == null)
                    break;
                else
                {
                    TapDCG(nElement);
                    Select(nElement);
                    break;
                }
            }
        }
        RenderObject();
    }
    public override bool Tap(Vector3 position)
    {
        float playerScale = GameObject.Find("Managers").GetComponent<SettingsManager>().playerScale;
        float selectDist = defaultSelectDistance * playerScale * 0.50f;
        DCGElement elem = selectElement(position, selectDist);
        return elem == null ? false : TapDCG(elem);
    }
    DCGElement selectElement(Vector3 pos, float selectDist)
    {
        if (type.Equals(types.Point))
            return DCGBase.NearestPoint(pos, selectDist);
        else if (type.Equals(types.Edge))
            return DCGBase.NearestEdge(pos, selectDist);
        else if (type.Equals(types.Face))
            return DCGBase.NearestFace(pos, selectDist);
        else
            return null;
    }
    public override bool TapDCG(DCGElement nearestElement)
    {
        if (nearestElement.ParentSelected())
        {
            return false;
        }
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
}
