using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptButton : HUDButton
 {
    public bool answer;
    public PluggableAction action;

    public override void OnPress()
    {
        if (answer && action != null)
            action.Action();
        Invoke("Exit", .5f);
    }

    void Exit()
    {
        HUDManager.hudManager.Pop();
    }
}
