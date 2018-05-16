using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPrefab : MonoBehaviour {

    // prefab is the new spawned prefab.
    // external prefab is the prefab that will be spawned on collision
    GameObject prefab;
    public GameObject externalPrefab;
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
        hUtil = new HandsUtil();
        leapcontroller = new Leap.Controller();
        frame = leapcontroller.Frame();
        HUDMgr = GameObject.Find("Managers").GetComponent<HUDManager>();
        dcg = GameObject.Find("DCG").GetComponent<DCGBase>();
    }

    // Update is called once per frame
    void Update()
    {
        frame = leapcontroller.Frame();
        // if there are hands visible in the view.
        if (frame.Hands.Count > 0)
        {
            lHand = frame.Hands[0];
            if (lHand.IsLeft)
            {
                // get the selected prefab from the hud manager and make it appear above the right index finger
                if (this.prefab){
                    prefab.transform.localPosition = new Vector3(rightIndex.transform.position.x, rightIndex.transform.position.y, rightIndex.transform.position.z) + rightIndex.transform.rotation*(new Vector3(0.0f,0.00f,0.04f));
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "bone3" || collision.gameObject.name == "bone2")
        {
            Leap.Unity.FingerModel finger = collision.gameObject.GetComponentInParent<Leap.Unity.FingerModel>();

            if (finger && finger.fingerType.ToString() == "TYPE_INDEX" && rightIndex)
            {
                // instantiate new prefab, set local position to be the palm position
                this.prefab = Instantiate(externalPrefab);
                this.prefab.transform.localPosition = new Vector3(LeapHandController.transform.position.x, LeapHandController.transform.position.y, LeapHandController.transform.position.z) + new Vector3(0.00f, 0.00f, 0.25f) + LeapHandController.transform.rotation * (new Vector3(0.0f, 0.00f, 0.04f));
                this.prefab.transform.localScale = new Vector3(.02f, .02f, .02f);
                // set color of spawned prefab to the current hud color
                this.prefab.gameObject.GetComponent<Renderer>().material.color = HUDMgr.getHUDColor();
                Mesh mesh = this.prefab.GetComponent<MeshFilter>().mesh;
                new Solid(mesh, Matrix4x4.TRS(prefab.transform.position, prefab.transform.rotation, prefab.transform.localScale), prefab.transform.position);
            }
        }
    }
}
