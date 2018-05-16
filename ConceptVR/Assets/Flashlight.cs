using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This behaviour sends its position to the glowing materials attached to it
public class Flashlight : MonoBehaviour {
    public List<Material> glowMats;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        foreach (Material mat in glowMats)
            mat.SetVector("_Center", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0));
    }
}
