using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemBase : NetworkBehaviour {
    public static ItemBase itemBase;

    public static List<Item> items;
    public static List<Item> sItems;
    public GameObject LightPrefab;
    public GameObject DoodlePrefab;
    public GameObject TextPrefab;
    public List<Material> materials;
    public static int defaultIndex = 0;
    public string firstType;
    public bool isHUD = false;
    public Material selectMat;

    // Use this for initialization
    void Start () {
        firstType = "";
        items = new List<Item>();
        sItems = new List<Item>();

        itemBase = this;
    }
	
	// Update is called once per frame
	void Update () {
        selectMat.SetFloat("_Offset", Time.time / 6f);
        if (sItems.Count == 0 && firstType != "")
            firstType = "";
    }
    public void Add(Item item)
    {
        items.Add(item);
        GameObject go = item.gameObject;
        NetPlayer.local.CmdSpawn(go);
        
    }
    //Spawns the object on the Server. This may be a bad function and can be handled in the specific tools.
    [Command]
    public void CmdSpawn(GameObject go)
    {
        NetworkServer.Spawn(go);
    }
    [ClientRpc]
    public void RpcSpawn(GameObject go)
    {
        Instantiate(go);
    }
    [Command]
    public void CmdDeSpawn(GameObject go)
    {
        NetworkServer.Destroy(go);
    }
    [ClientRpc]
    public void RpcDeSpawn(GameObject go)
    {
        Destroy(go);
    }
    public void Remove(Item item)
    {
        items.Remove(item);
        CmdDeSpawn(item.gameObject);
    }
    public Item findNearestItem(Vector3 position)
    {
        Item nearestItem = null;
        float nearestDistance = 99999;
        float maxDistance = 0.1f;
        foreach (Item item in items)
        {
            float distance = Vector3.Distance(position, item.Position(position));
            if (distance < nearestDistance && distance < maxDistance)
            {
                nearestItem = item;
                nearestDistance = distance;
            }
        }
        // TODO: same for text, or any other item that we add in the future

        return nearestItem;
    }
    //Not gonna lie this is pretty ghetto. It works though so i dunno
    public void itemHudManager(Item item)
    {
        //Could use a a stack to define the items on then stack, however may be more intrusive.
        if(firstType == "")
        {
            firstType = item.GetType().ToString();
            item.Push();
            isHUD = true;
            return;
        }
        if(firstType != item.GetType().ToString())
        {
            Item.Pop();
            isHUD = false;
        }
    }
    public static void changeIndex(int index)
    {
        ItemBase.defaultIndex = index;
    }
    //Return the nearest item to a position.
    public static Item NearestItem(Vector3 pos, float maxDist)
    {
        Item nItem = null;
        foreach (Item item in ItemBase.items)
        {
            if (item.GetType() == typeof(Doodle))
                continue;
            float dist = item.Distance(pos);
            if (dist < maxDist)
            {
                maxDist = dist;
                nItem = item;
            }
        }

        return nItem;
    }

}
