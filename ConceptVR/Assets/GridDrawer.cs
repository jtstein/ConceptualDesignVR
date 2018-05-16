using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour {
    public Material glowMat;
    Vector3 center;
    public GameObject target;

    List<LineRenderer> lines;

	// Use this for initialization
	void Start () {
        lines = new List<LineRenderer>();
        if (SettingsManager.sm != null)
            SetScale(SettingsManager.sm.gridSnap);
        else
            SetScale(1f / 16f);
	}
	
	// Update is called once per frame
	void Update () {
        if (target != null)
            SetCenter(target.transform.position);
	}

    public void SetScale(float scale)
    {
        foreach (LineRenderer line in lines)
            Destroy(line.gameObject);
        lines = new List<LineRenderer>();
        for (float u = -.075f/scale; u <= .075f / scale; ++u)
            for (float v = -.075f / scale; v <= .075f / scale; ++v)
            {
                float a = u * scale;
                float b = v * scale;
                float c = .075f;
                addGridLine(new Vector3(a, b, c), new Vector3(a, b, -c), scale);
                addGridLine(new Vector3(a, c, b), new Vector3(a, -c, b), scale);
                addGridLine(new Vector3(c, a, b), new Vector3(-c, a, b), scale);
            }
    }

    GameObject addGridLine(Vector3 pos1, Vector3 pos2, float scale)
    {
        GameObject lrg = new GameObject();
        lrg.transform.parent = transform;
        LineRenderer lr = lrg.AddComponent<LineRenderer>();
        Vector3[] pArr = new Vector3[2];
        pArr[0] = pos1;
        pArr[1] = pos2;
        lr.positionCount = 2;
        lr.SetPositions(pArr);
        lr.material = glowMat;
        lr.startWidth = scale/20f;
        lr.endWidth = scale/20f;
        lr.useWorldSpace = false;

        lines.Add(lr);
        return lrg;
    }

    public void SetCenter(Vector3 pos)
    {
        center = pos;
        transform.position = SettingsManager.sm.snapToGrid(pos);
        glowMat.SetVector("_Center", new Vector4(center.x, center.y, center.z, 0));
    }
}
