using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public static List<Grabbable> all = new List<Grabbable>();

    [HideInInspector] public Vector3 startGrabPos;
    [HideInInspector] public Vector3 startGrabDiff;
    [HideInInspector] public Quaternion startGrabOri;
    [HideInInspector] public Tool grabbingTool;

    public float grabRadius = .025f;


    protected void Start()
    {
        all.Add(this);
    }

    protected void Update()
    {
        if (grabbingTool != null)
        {
            transform.position = grabbingTool.getPos() + startGrabDiff;
            whileGrabbed();
        }
    }
    
    public void Grab(Tool grabber)
    {
        grabbingTool = grabber;
        Debug.Log(grabber);
        startGrabDiff = transform.position - grabbingTool.getPos();
        startGrabPos = transform.position;

        OnGrab();
    }

    public void Release()
    {
        OnRelease();
        grabbingTool = null;
    }

    public virtual void OnGrab() { }
    public virtual void OnRelease() { }
    public virtual void whileGrabbed() { }
}
