using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGrapher : MonoBehaviour {
    public class Graph
    {
        struct GraphValue
        {
            public float value;
            public bool isPOI;
            public GraphValue(float value, bool isPOI)
            {
                this.value = value;
                this.isPOI = isPOI;
            }
        }

        public LineRenderer line;
        Queue<GraphValue> values;
        public float yScale;
        public float xScale;
        public float zOffset;

        public Graph(float x, float y)
        {
            xScale = x;
            yScale = y;

            values = new Queue<GraphValue>();
        }

        public void Push(float value, bool isPOI, int length)
        {
            values.Enqueue(new GraphValue(value, isPOI));
            if (values.Count > length)
                values.Dequeue();
        }

        public void Update()
        {
            Vector3[] positions = new Vector3[values.Count];
            Vector3 cPos = line.transform.position;
            int i = 0;
            foreach (GraphValue gv in values)
            {
                positions[i] = cPos + new Vector3(((float)i / (values.Count - 1)) * xScale, gv.value * yScale, zOffset);
                ++i;
            }
            line.SetPositions(positions);
        }

        public void Render()
        {
            int i = 0;
            foreach (GraphValue gv in values)
            {
                if (gv.isPOI)
                    Graphics.DrawMeshNow(GeometryUtil.icoSphere2, Matrix4x4.TRS(new Vector3(((float)i / (values.Count - 1)) * xScale, gv.value * yScale, zOffset) + line.transform.position, Quaternion.identity, Vector3.one * 0.002f));
                ++i;
            }
        }
    }

    Dictionary<string, Graph> graphs;

    public List<string> graphNames;
    public Material material;
    public int length;
    public float xScale;
    public float yScale;

    //Initiaize
	void Start () {
        //populate graphs
        graphs = new Dictionary<string, Graph>();
        foreach (string k in graphNames)
            graphs.Add(k, new Graph(xScale, yScale));

        int i = 0;
        foreach (KeyValuePair<string, Graph> kv in graphs)
        {
            for (int j = 0; j < length; ++j)
                kv.Value.Push(0, false, length);
            GameObject line = new GameObject("DebugLine");
            line.transform.position = this.transform.position;
            line.transform.SetParent(this.transform);
            LineRenderer ren = line.AddComponent<LineRenderer>();
            ren.material = material;
            ren.positionCount = length;
            ren.startWidth = 0.005f;
            ren.endWidth = 0.005f;
            Graph g = kv.Value;
            g.line = ren;
            g.zOffset = i * 0.02f;

            ++i;
        }
	}

    void Update() {
        foreach (KeyValuePair<string, Graph> kv in graphs)
            kv.Value.Update();
    }

    public void AddValue(string paramName, float value, bool isPOI)
    {
        graphs[paramName].Push(value, isPOI, length);
    }

    private void OnRenderObject()
    {
        material.SetPass(0);
        foreach (KeyValuePair<string, Graph> kv in graphs)
            kv.Value.Render();
    }
}
