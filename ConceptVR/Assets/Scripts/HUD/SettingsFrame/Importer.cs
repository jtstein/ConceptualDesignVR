using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public class Importer : HUDButton {

    // prefab is the new spawned prefab.
    // external prefab is the prefab that will be spawned on collision
    GameObject prefab;
    GameObject externalPrefab;
    DCGBase dcg;

    HUDManager HUDMgr;
    HandsUtil hUtil;

    Leap.Controller leapcontroller;
    Leap.Frame frame;
    Leap.Hand lHand;

    public GameObject LeapHandController;

    // Use this for initialization
    void Start()
    {
        externalPrefab = new GameObject();
        prefab = new GameObject();
        hUtil = new HandsUtil();
        leapcontroller = new Leap.Controller();
        frame = leapcontroller.Frame();
        HUDMgr = GameObject.Find("Managers").GetComponent<HUDManager>();
        dcg = GameObject.Find("DCG").GetComponent<DCGBase>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        frame = leapcontroller.Frame();
        // if there are hands visible in the view.
        if (frame.Hands.Count > 0)
        {
            lHand = frame.Hands[0];
            // If it is the left hand, make the position of the HUD relative to the local position of the right index(bone3).
            if (lHand.IsLeft)
            {
                // get the selected prefab from the hud manager and make it appear above the right index finger
                if (this.prefab)
                {
                }
            }
        }
        */
        base.Update();
    }

    public override void OnPress()
    {
        string fileName = transform.Find("Text").GetComponent<TextMesh>().text;

        Vector3 pos = Hands.Left.PalmPosition.ToVector3();
        GameObject loadedObj = OBJLoader.LoadOBJFile("Assets/Resources/Imports/Meshes/" + fileName);
        this.prefab = loadedObj;
        loadedObj.transform.position = transform.position = new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.00f, 0.00f, 0.25f); 
        loadedObj.transform.localScale = new Vector3(.02f, .02f, .02f);
        this.prefab = Instantiate(loadedObj);


        this.prefab.transform.position = new Vector3(pos.x, pos.y, pos.z) + new Vector3(0.00f, 0.00f, 0.25f); //+ new Vector3(Hands.Left.Rotation.x, Hands.Left.Rotation.y, Hands.Left.Rotation.z);
        //LeapHandController.transform.rotation * (new Vector3(0.0f, 0.00f, 0.04f));
        this.prefab.transform.localScale = new Vector3(.02f, .02f, .02f);
        this.prefab.gameObject.name = fileName;

        new Solid(this.prefab.GetComponent<MeshFilter>().mesh, Matrix4x4.TRS(prefab.transform.position, prefab.transform.rotation, prefab.transform.localScale), prefab.transform.position);

        base.OnPress();
    }


}
