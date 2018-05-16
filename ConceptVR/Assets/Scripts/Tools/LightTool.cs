using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTool : Tool {

    ItemBase itemBase;
    GameObject Managers;

    // Use this for initialization
    void Start () {
        itemBase = GameObject.Find("ItemBase").GetComponent<ItemBase>();
        Managers = GameObject.Find("Managers");
        //LightPrefab.GetComponent<Light>().color = LightPrefab.GetComponent<Material>().color;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public override bool Tap(Vector3 position)
    {
        GameObject obj = Instantiate(itemBase.LightPrefab, controllerPosition, new Quaternion(0, 0, 0, 0));
        float playerScale = Managers.GetComponent<SettingsManager>().playerScale;
        obj.transform.localScale = new Vector3(playerScale, playerScale, playerScale)/50;
        ItemBase.itemBase.Add(obj.GetComponent<LightItem>());
        return true;
    }
}
