using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GenerateImportButtons : ToggleButton
{

    public GameObject ImportListButton;

    List<GameObject> importButtons;
    // prefab is the new spawned prefab.
    DCGBase dcg;
    // right index finger for tracking position of spawned prefab
    public GameObject rightIndex;

    HUDManager HUDMgr;
    HandsUtil hUtil;

    Leap.Controller leapcontroller;
    Leap.Frame frame;
    Leap.Hand lHand;

    public GameObject LeapHandController;

    // Use this for initialization
    void Start()
    {
        this.importButtons = GenerateImportList();
        hUtil = new HandsUtil();
        leapcontroller = new Leap.Controller();
        frame = leapcontroller.Frame();
        HUDMgr = GameObject.Find("Managers").GetComponent<HUDManager>();
        dcg = GameObject.Find("DCG").GetComponent<DCGBase>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        frame = leapcontroller.Frame();
        // if there are hands visible in the view.
        if (frame.Hands.Count > 0)
        {
            lHand = frame.Hands[0];
            // If it is the left hand, make the position of the HUD relative to the local position of the right index(bone3).
            if (lHand.IsLeft)
            {
                // get the selected prefab from the hud manager and make it appear above the right index finger
               // if (this.prefab)
                //{
                 //   prefab.transform.localPosition = new Vector3(rightIndex.transform.position.x, rightIndex.transform.position.y, rightIndex.transform.position.z) + rightIndex.transform.rotation * (new Vector3(0.0f, 0.00f, 0.04f));
               // }
            }
        }
    }

    public override void ToggleOn()
    {
       // this.OnPress();
    }

    public override void OnPress()
    {
        foreach (GameObject btn in this.importButtons)
        {
            if (this.toggled)
            {
                btn.gameObject.SetActive(false);
            }
            else
            {
                btn.gameObject.SetActive(true);
            }
        }
        base.OnPress();
    }

    public List<GameObject> GenerateImportList()
    {
        List<GameObject> importList = new List<GameObject>();
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Imports/Meshes/");
        FileInfo[] info = dir.GetFiles("*.obj");

        Vector3 btnRelPosition = new Vector3(0.125f, 0.075f, 0.075f);
        foreach (FileInfo f in info)
        {
            // generate new ImportListButton for each import available
            GameObject importBtn = Instantiate(ImportListButton);
            importBtn.transform.parent = this.transform;
            importBtn.transform.Find("Text").GetComponent<TextMesh>().text = f.Name;
            importBtn.gameObject.name = f.Name + "_Button";
            btnRelPosition -= new Vector3(0.0f, 0.0f, 0.035f);
            //importBtn.gameObject.transform.localPosition = btnRelPosition;
            importBtn.gameObject.GetComponent<HUDButton>().endPos = btnRelPosition;
            importList.Add(importBtn);
            importBtn.gameObject.SetActive(false);
        }
        return importList;
    }
}
