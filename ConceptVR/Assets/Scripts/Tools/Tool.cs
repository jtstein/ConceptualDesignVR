using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public abstract class Tool : MonoBehaviour {
    public bool triggerInput;
    public bool gripInput;
    public bool formInput;
    public Vector3 controllerPosition;
    public Vector3 forward;
    public int playerID;
    public VideoClip videoClip;
    public void setPos(Vector3 pos)
    {
        controllerPosition = pos;
    }
    public Vector3 getPos()
    {
        return controllerPosition;
    }

    public virtual void Update()
    {
        transform.position = controllerPosition;
        forward = transform.forward;
    }

    public virtual bool TriggerDown() { return false; }
    public virtual bool TriggerUp() { return false; }
    public virtual bool DualTriggerDown() { return false; }
    public virtual bool DualTriggerUp() { return false; }
    public virtual bool GripDown() { return false; }
    public virtual bool GripUp() { return false; }
    public virtual bool Tap(Vector3 position) { return false; }
    public virtual bool Swipe() { return false; }
    public virtual void FreeForm(LeapTrackedController ltc) {}
    public virtual void FreeFormEnd() {}
    public virtual bool Fire() { return false; }
}
