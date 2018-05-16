using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDSlider : HUDButton {
    public float friction;
    public Vector3 positiveRange;
    public float defaultValue;
    public List<float> snapPoints;
    public float snapRange;

    [HideInInspector] public float value;
    float targetValue;
    Vector3 localPressPos;
    GameObject sliderObject;
    bool touching;

    void Start()
    {
        base.Start();
        value = defaultValue;
        sliderObject = transform.Find("SlidingView").gameObject;
    }

    void Update () {
        base.Update();
        if (touching)
        {
            localPressPos = transform.InverseTransformPoint(fingerTip.transform.position);
            targetValue = Vector3.Project(localPressPos + positiveRange, positiveRange).magnitude / positiveRange.magnitude / 2;
            if (Vector3.Dot(localPressPos + positiveRange, positiveRange) < 0)
                targetValue = 0;
            targetValue = Mathf.Clamp01(targetValue);

            int nearestSnap = -1;
            float nearestDist = snapRange;
            for(int i = 0; i < snapPoints.Count; ++i)
            {
                float diff = Mathf.Abs(targetValue - snapPoints[i]);
                if (diff < nearestDist)
                {
                    nearestDist = diff;
                    nearestSnap = i;
                }
            }

            if (nearestSnap != -1)
                targetValue = snapPoints[nearestSnap];
        }
        value = (value * friction + targetValue) / (1+friction);
        sliderObject.transform.localPosition = (value * 2 - 1) * positiveRange;
	}

    public override void OnPress()
    {
        touching = true;
    }

    public override void OnRelease()
    {
        touching = false;
    }
}
