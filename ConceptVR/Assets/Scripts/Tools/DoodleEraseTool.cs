using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoodleEraseTool : Tool {
    public Material destMat;

    float delDist = 0.031456284f;

    // Use this for initialization
    void Start()
    {

    }

    new private void Update()
    {
        if (triggerInput)
        {
            List<Doodle> newDoods = new List<Doodle>();
            foreach (Doodle d in ItemBase.items)
            {
                if (!d.destroyed)
                    newDoods.AddRange(d.eraseSphere(controllerPosition, delDist));
            }

            foreach (Doodle d in newDoods)
                ItemBase.items.Add(d);
        }
    }

    private void OnRenderObject()
    {
        float playerScale = GameObject.Find("Managers").GetComponent<SettingsManager>().playerScale;
        if (triggerInput)
        {
            destMat.SetPass(0);
            Graphics.DrawMeshNow(GeometryUtil.icoSphere4, Matrix4x4.TRS(controllerPosition, Quaternion.identity, new Vector3(delDist, delDist, delDist) * playerScale));
        }
    }
}
