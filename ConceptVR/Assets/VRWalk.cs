using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRWalk : MonoBehaviour {
    GameObject vrCage;
    Transform head;
	// Use this for initialization
	void Start () {
        vrCage = GameObject.Find("[CameraRig]");
        head = GameObject.Find("[CameraRig]").transform.Find("Camera (head)");
	}
	
	// Update is called once per frame
	void Update () {
        vrCage.transform.position += Quaternion.Euler(0, head.rotation.eulerAngles.y, 0) * (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * Time.deltaTime;
	}
}
