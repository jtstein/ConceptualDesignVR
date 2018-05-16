using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDTool : Tool {
    HUDManager HUD;

    //TODO: change to PalmOpened
    public override bool TriggerDown()
    {
        HUD.HUDObject.SetActive(true);
        return true;
    }

    //TODO: change to PalmClosed
    public override bool TriggerUp()
    {
        HUD.HUDObject.SetActive(false);
        return true;
    }
}
