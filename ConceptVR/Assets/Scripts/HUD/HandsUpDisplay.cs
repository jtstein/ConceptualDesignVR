using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class HandsUpDisplay : MonoBehaviour
{
    Leap.Controller leapcontroller;
    Leap.Frame frame;
    Leap.Hand lHand;
	bool placed;
    public GameObject LeapHandController;
	public AnchorButton anchor;

    // Use this for initialization
    void Start()
    {
        leapcontroller = new Leap.Controller();
		placed = false;
	}

    // Update is called once per frame
    void Update()
    {

        frame = leapcontroller.Frame();

        // if there are hands visible in the view.
        if (frame.Hands.Count > 0 && !placed)
        {
            lHand = frame.Hands[0];
            // If it is the left hand, make the position of the HUD relative to the local position of the palm.
            if (lHand.IsLeft)
            {

                // WORKS ON TESTSCENE
                transform.localPosition = new Vector3(lHand.PalmPosition.x, lHand.PalmPosition.y, -lHand.PalmPosition.z) / 1000f + new Vector3(-0.0f, -1.05f, -1.20f) ;
                //transform.rotation = Quaternion.FromToRotation(Vector3.up, lHand.PalmNormal.ToVector3);

                // THIS ONE WORKS ON THE CLOCK SCENE
                //transform.localPosition = new Vector3(lHand.PalmPosition.x, lHand.PalmPosition.y, -lHand.PalmPosition.z) / 1000f + new Vector3(-0.0f, -1.15f, -1.15f);
            }
        }
    }
}