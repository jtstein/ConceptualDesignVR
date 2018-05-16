using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDButton : MonoBehaviour {
    Animator animator;

    protected HUDManager HUD;
    static float coolDown = 0.5f;

    public GameObject startControl;
    public GameObject endControl;
    public Vector3 startPos;
    public Vector3 endPos;
    public float lerpAmt;
    public float hoverHgt;

    protected GameObject fingerTip;
    static float lastPressTime;

    public void Start()
    {
        gameObject.layer = 8;
        GameObject MgrObject = GameObject.Find("Managers");
        if (MgrObject != null)
            HUD = MgrObject.GetComponent<HUDManager>();

        animator = gameObject.GetComponent<Animator>();
        if (endControl == null && endPos == Vector3.zero)
            endPos = transform.localPosition;
    }

    public void Update()
    {
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (startControl != null)
            startPos = startControl.transform.localPosition;
        if (endControl != null)
            endPos = endControl.transform.localPosition;
    
        transform.localPosition = Vector3.LerpUnclamped(startPos, endPos, lerpAmt) + Vector3.up * hoverHgt;
    }

    public virtual void OnPress() {}

    public virtual void OnRelease() {}

    public void OnEnable()
    {
        if (animator == null)
            Start();
        animator.Play("Enable");
        UpdatePosition();
    }

    public void OnDisable()
    {
        animator.Play("Idle");
        lerpAmt = 0;
        hoverHgt = 0;
        UpdatePosition();
    }

    public void OnSubTriggerEnter(Collider other, string triggerName)
    {
            Debug.Log(other);
        if (triggerName == "PressCollider" && Time.time - lastPressTime > coolDown)
        {
            animator.Play("Press");
            fingerTip = other.gameObject;
            OnPress();
        } else if (triggerName == "HoverCollider")
        {
            animator.SetBool("Hovering", true);
        }
    }

    public void OnSubTriggerExit(Collider other, string triggerName)
    {
        if (triggerName == "PressCollider")
        {
            lastPressTime = Time.time;
            OnRelease();
        }
        else if (triggerName == "HoverCollider")
        {
            animator.SetBool("Hovering", false);
        }
    }
}
