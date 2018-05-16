using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class liftBox : MonoBehaviour {

    public GameObject box;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collision)
    {
        box.gameObject.GetComponent<Animator>().Play("testboxlift");
    }
}
