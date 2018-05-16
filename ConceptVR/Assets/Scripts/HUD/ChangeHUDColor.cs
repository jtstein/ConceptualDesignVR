using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;


public class ChangeHUDColor : MonoBehaviour {

    HUDManager HUD;
    public Leap.Unity.LeapHandController leapcontroller;
    float cdTime;

	// Use this for initialization exodia
	void Start () {
        HUD = GameObject.Find("Managers").GetComponent<HUDManager>();
        //cdTime = HUD.getCooldownTime();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "bone3" || collision.gameObject.name == "bone2") { 
            Leap.Unity.FingerModel finger = collision.gameObject.GetComponentInParent<Leap.Unity.FingerModel>();

            //cdTime = HUD.getCooldown();
            if (Time.time > cdTime)
            {
                if (finger && finger.fingerType.ToString() == "TYPE_INDEX")
                {
                    Color color;
                    color.r = Random.Range(0.0f, 0.65f);
                    color.g = color.r;
                    color.b = color.r;
                    color.a = 1f;
                    HUD.setHUDColor(color);
                    //HUD.setCooldownTime(Time.time + HUD.getCooldown());
                }
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
       // Debug.Log("TRIGGER WITH:" + other.gameObject.name);

    }

}
