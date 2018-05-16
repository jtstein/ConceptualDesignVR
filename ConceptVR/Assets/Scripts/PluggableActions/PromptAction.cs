using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Action", menuName = "Actions/PromptAction")]
public class PromptAction : PluggableAction {
    public HUDFrame promptFrame;
    public Vector3 position;

    public override void Action()
    {
        HUDManager.hudManager.Push(promptFrame);
        promptFrame.transform.position = position;
    }
}
