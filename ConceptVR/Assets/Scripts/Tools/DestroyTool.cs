using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTool : Tool {
    public Material destMat;

    float delDist = 0.05f;

	// Use this for initialization
	void Start () {

	}

    new private void Update()
    {
        if (triggerInput)
        {
            Point nPoint;
            Item nItem;
            while (true)
            {
                float playerScale = GameObject.Find("Managers").GetComponent<SettingsManager>().playerScale;
                nPoint = DCGBase.NearestPoint(controllerPosition, delDist*playerScale);
                nItem = ItemBase.NearestItem(controllerPosition, delDist * playerScale);
                if (nPoint == null && nItem == null)
                    break;
                else if (nItem == null)
                    Remove(nPoint);
                else if (nPoint == null)
                    ItemBase.itemBase.Remove(nItem);
                else
                    Remove(nPoint); ItemBase.itemBase.Remove(nItem);


            }
        }
    }

    private void OnRenderObject()
    {
        float playerScale = GameObject.Find("Managers").GetComponent<SettingsManager>().playerScale;
        if (triggerInput)
        {
            destMat.SetPass(0);
            Graphics.DrawMeshNow(GeometryUtil.icoSphere4, Matrix4x4.TRS(controllerPosition, Quaternion.identity, new Vector3(delDist, delDist, delDist)*playerScale));
        }
    }
    public void Remove(Point p)
    {
        if (p.isSelected && p.isLocked)
            return;
        p.Remove();
    }
}
