using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : HUDButton {
    NetworkStarter starter;
    public void Start()
    {
        base.Start();
        starter = GameObject.Find("NetworkStarter").GetComponent<NetworkStarter>();
    }
    public override void OnPress()
    {
        starter.Restart();
        DCGBase.RemoveAll();
        SceneManager.LoadScene("Scenes/Network Testing");
    }
}
