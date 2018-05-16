using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDFrame: MonoBehaviour
{
    public bool isSubFrame = false;
    public bool loose = false;
    enum animationState { In, Out, Idle };
    List<HUDView> viewList;
}