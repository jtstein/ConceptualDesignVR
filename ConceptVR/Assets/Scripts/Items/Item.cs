using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Item : NetworkBehaviour {
    [SyncVar(hook = "OnSelected")]
    public bool isSelected;
    [SyncVar]
    public bool isLocked;
    public bool destroyed = false;
    protected static HUDManager HUD;
    public Material oldMat;
    public static bool popped = true;
    // Use this for initialization
    protected void Start() {
        HUD = GameObject.Find("Managers").GetComponent<HUDManager>();
        ItemBase.items.Add(this);
    }

    // Update is called once per frame
    void Update() {

    }
    #region Override Functions
    public virtual float Distance(Vector3 pos) { return Mathf.Infinity; }
    public virtual void CmdSelect()
    {
        oldMat = this.gameObject.GetComponent<Renderer>().material;
        this.gameObject.GetComponent<Renderer>().material = ItemBase.itemBase.selectMat;
    }
    public virtual void CmdDeSelect()
    {
        this.gameObject.GetComponent<Renderer>().material = oldMat;
        if (destroyed) return;
    }
    public virtual void Push() { popped = false; }
    public virtual void changeColor(Color color) { }
    public virtual Vector3 Position(Vector3 contPos) { return new Vector3(); }
    public virtual void SelectUtil() { }
    public virtual Vector3 Position() { return this.gameObject.transform.position; }
    public virtual void changePosition(Vector3 start, Vector3 contr, Vector3 hold)
    {
        this.gameObject.transform.position = (start + contr - hold);
    }
    #endregion
    public static void Pop()
    {
        if (!popped)
        {
            popped = !popped;
            HUD.Pop();
        }
    }
    public void OnDestroy()
    {
        ItemBase.items.Remove(this);
    }
    public void OnSelected(bool boolean) { this.SelectUtil(); }
}
