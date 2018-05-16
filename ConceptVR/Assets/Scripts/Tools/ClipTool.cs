using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipTool : MoveTool {
    
    new void Update()
    {
        base.Update();
    }

    public override bool DualTriggerDown()
    {
        foreach (Solid clipper in DCGBase.sElements)
            foreach (Solid target in DCGBase.solids)
                if (!DCGBase.sElements.Contains(target))
                    ClipUtil.Clip(target, clipper, ClipUtil.ClipMode.Subtract);
        Swipe();
        return true;
    }
}
