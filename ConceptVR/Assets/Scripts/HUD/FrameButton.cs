using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameButton : HUDButton {
    
    public HUDFrame targetFrame;

    public bool isSubFrameButton = true;

    public override void OnPress()
    {
        if (!this.isSubFrameButton)
        {
            HUD.popAll();
        }
        HUD.Push(targetFrame);
    }
}
