using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        List<Point> points = new List<Point>();
        points.Add(new Point(Random.onUnitSphere * 5f));
        points.Add(new Point(Random.onUnitSphere * 5f));
        points.Add(new Point(Random.onUnitSphere * 5f));
        points.Add(new Point(Random.onUnitSphere * 5f));
        points.Add(new Point(Random.onUnitSphere * 5f));

        Edge e = new Edge(points, false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
