using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameToolButton : SwapToolButton {
    public HUDFrame targetFrame;

    public bool isSubFrameButton = true;

    public override void OnPress()
    {
        if (!this.isSubFrameButton)
        {
            HUD.Pop();
        }
        HUD.Push(targetFrame);
        base.OnPress();
    }
}
