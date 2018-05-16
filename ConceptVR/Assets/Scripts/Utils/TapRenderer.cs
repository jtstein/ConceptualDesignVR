using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapRenderer : MonoBehaviour {
    float size;
    public Material mat;

	// Use this for initialization
	void Start () {
        size = 0;
        Destroy(gameObject, .5f);
	}
	
	// Update is called once per frame
	void Update () {
        size += Time.deltaTime / 5f;
	}

    private void OnRenderObject()
    {
        mat.SetPass(0);
        Graphics.DrawMeshNow(GeometryUtil.cylinder32, Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(size, 0.001f, size)));
    }
}
