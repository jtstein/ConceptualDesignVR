using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* Networking for doodles only works from server to client
 * not in reverse. To fix this issue the doodle updating code needs to be moved into 
 * a class or object with local authority. An example would be NetPlayer.
 * Currently exists in Item base. Move code and it should work.
 */ 
public class Doodle : Item {
    public Bounds boundingBox;
    public LineRenderer lr;
    int numClicks = 0;
    //[SyncVar (hook = "Encapsulate")]
    //public Vector3 latestPoint;
    //Sync var to syncronize doodle variables. 
    [SyncVar(hook = "finalBounds")]
    public bool isFinished;
    [SyncVar(hook = "changeWidth")]
    public float lineWidth;
    List<Vector3> startPos;
    public int colorIndex = 0;
	// Use this for initialization
	void Start () {
        lr = this.gameObject.GetComponent<LineRenderer>();
        colorIndex = ItemBase.defaultIndex;
        CmdChangeColor(colorIndex);
        isFinished = false;
        base.Start();
        startPos = new List<Vector3>();
    }
	
	// Update is called once per frame
	void Update () {
	}
    //Retrieves the closest vector3, to a defined position(controller pos)
    //Line renderer's contain n vector 3's.
    public override Vector3 Position(Vector3 contPos)
    {
        int lowestPos = 0;
        float lowDist = Mathf.Abs(Vector3.Distance(lr.GetPosition(0), contPos));
        for (int i = 1; i < lr.positionCount; ++i)
        {
            float newDist = Mathf.Abs(Vector3.Distance(lr.GetPosition(i), contPos));
            if (newDist < lowDist)
            {
                lowestPos = i;
                lowDist = newDist;
            }
        }
        return lr.GetPosition(lowestPos);
    }
    //deprecated
    public void erasePoint(Vector3 p)
    {
        Vector3[] verts = new Vector3[lr.positionCount];
        lr.GetPositions(verts);

        int index = -1;
        for (int i = 0; i < lr.positionCount; ++i)
        {
            if (verts[i] == p)
            {
                index = i;
                break;
            }
        }
        if (index == -1)
            Debug.LogError("Attempted to erase non-existent point " + p);
        else if (index <= 1)    //front end
        {
            for (int i = index; i < lr.positionCount - index - 1; ++i)
                verts[i] = verts[i + index + 1];
            lr.positionCount -= (index + 1);
        }
        else if (index >= lr.positionCount - 1) //back end
        {
            lr.positionCount = index;
        }
        else //somewhere in the middle
        {
            List<Vector3> newDoodPositions = new List<Vector3>();
            for (int i = index; i < lr.positionCount - 1; ++i)
            {
                newDoodPositions.Add(verts[i]);
            }
            lr.positionCount = index - 1;

            Doodle newDood = Instantiate(ItemBase.itemBase.DoodlePrefab).GetComponent<Doodle>();
            newDood.CmdSetPoints(newDoodPositions.ToArray());
        }
    }
    //See Destroy tool for simpler destroy understanding.
    //Doodles when destroyed spolit into multiples, thus new doodles must be 
    //recreated dynamically.
    public List<Doodle> eraseSphere(Vector3 center, float radius)
    {
        List<List<Vector3>> segs = new List<List<Vector3>>();
        Vector3[] positions = new Vector3[lr.positionCount];
        lr.GetPositions(positions);

        bool pErase = Vector3.Distance(positions[0], center) < radius;
        bool initErase = pErase;
        segs.Add(new List<Vector3>());
        segs[segs.Count - 1].Add(positions[0]);
        //Find points in the doodle to be erased.
        for (int i = 1; i < positions.Length; ++i)
        {
            bool iErase = Vector3.Distance(positions[i], center) < radius;
            if (iErase == pErase)
                segs[segs.Count - 1].Add(positions[i]);
            else
            {
                Vector3 orig = pErase ? positions[i - 1] : positions[i];
                Vector3 dir = pErase ? positions[i] - positions[i - 1] : positions[i-1] - positions[i];
                Vector3 hit = GeometryUtil.raySphereHit(orig, dir, center, radius);
                segs[segs.Count - 1].Add(hit);
                segs.Add(new List<Vector3>());
                segs[segs.Count - 1].Add(hit);
                segs[segs.Count - 1].Add(positions[i]);
            }

            pErase = iErase;
        }
        if (segs.Count == 1) return new List<Doodle>();
        List<Doodle> doods = new List<Doodle>();
        ItemBase.changeIndex(this.colorIndex);
        //Create new doodles from where the doodle was split.
        foreach (List<Vector3> seg in segs)
        {
            if (!initErase)
            {
                Doodle dood = Instantiate(ItemBase.itemBase.DoodlePrefab).GetComponent<Doodle>();
                dood.gameObject.SetActive(true);
                dood.lr = dood.GetComponent<LineRenderer>();           
                ItemBase.itemBase.Add(dood);
                dood.CmdSetPoints(seg.ToArray());
                doods.Add(dood);
                //ItemBase.items.Add(dood);
            }
            initErase = !initErase;
        }
        destroyed = true;
        ItemBase.itemBase.Remove(this);

        return doods;
    }
    //Set points on the client
    [ClientRpc]
    public void RpcSetPoints(Vector3[] points)
    {
        lr.positionCount = points.Length;
        lr.SetPositions(points);
    }
    //Set points on the server
    [Command]
    public void CmdSetPoints(Vector3[] points)
    {
        RpcSetPoints(points);
    }

    public override void CmdSelect()
    {
        oldMat = this.gameObject.GetComponent<LineRenderer>().material;
        this.gameObject.GetComponent<LineRenderer>().material = ItemBase.itemBase.selectMat;
        isLocked = true;
        isSelected = true;
    }
    public override void CmdDeSelect()
    {
        this.gameObject.GetComponent<LineRenderer>().material = oldMat;
        base.CmdDeSelect();
        isLocked = false;
        isSelected = false;
    }
    public void Encapsulate(Vector3 pos)
    {
        boundingBox.Encapsulate(pos);
    }
    public void finalBounds(bool boolean)
    {
        boundingBox.SetMinMax(boundingBox.min - new Vector3(.1f, .1f, .1f), boundingBox.max + new Vector3(.1f, .1f, .1f));
    }
    //Checks to see if a positrion is within the bounding box of the doodle.
    public override float Distance(Vector3 pos)
    {
        if (boundingBox.Contains(pos))
            return Vector3.Distance(pos, this.Position(pos));
        return 100000f;
    }
    public void changeWidth(float width)
    {
        lr.startWidth = width;
        lr.endWidth = width;
    }
    [Command]
    public void CmdUpdateLineRenderer(Vector3 pos)
    {
        RpcUpdateLR(pos);
    }
    [ClientRpc]
    public void RpcUpdateLR(Vector3 pos)
    {
        if (lr == null) return;
        lr.positionCount = numClicks + 1;
        lr.SetPosition(numClicks, pos);
        Encapsulate(pos);
        numClicks++;
    }
    //Deprecated. Used for testing
    public override void SelectUtil()
    {
        /*if (!isSelected)
        {
            oldColor = lr.material.color;
            lr.material.color = Color.white;
        }
        else
            lr.material.color = oldColor;*/
    }
    //Pushes doodle frame when only doodles are selected
    public override void Push()
    {
        base.Push();
        Debug.Log("Pushing Doodle Frame");
        GameObject frame = GameObject.Find("Frames");
        if (HUD != null && frame != null)
            HUD.Push(frame.transform.Find("DoodleFrame").gameObject.GetComponent<HUDFrame>());
    }
    //Changes colors on the client
    [ClientRpc]
    public void RpcChangeColor(int index)
    {
        lr.material = ItemBase.itemBase.materials[index];
        oldMat = ItemBase.itemBase.materials[index];
        Debug.Log("Changing to " + oldMat);
        colorIndex = index;
    }
    [Command]
    public void CmdChangeColor(int index)
    {
        RpcChangeColor(index);
    }
    //changes the positions of all vector 3's of a doodle. Used when moving doodles
    public override void changePosition(Vector3 start, Vector3 contr, Vector3 hold)
    {
        Debug.Log("boos");
        List<Vector3> newP = new List<Vector3>();
        for (int i = 0; i < lr.positionCount; ++i)
            newP.Add(startPos[i] + contr - hold);
        lr.SetPositions(newP.ToArray());
    }
    public override Vector3 Position()
    {
        startPos.Clear();
        for (int i = 0; i < lr.positionCount; ++i)
            startPos.Add(lr.GetPosition(i));
        return base.Position();
    }
}
