using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : HUDButton {
    public override void OnPress()
    {
        HUD.Pop();
    }
}
