using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTool : Tool {

    public GameObject laserPrefab;
    private GameObject laser;
    private Vector3 hitPoint;
    public GameObject rightfinger;

    GameObject LMHeadMountedRig;
    public GameObject teleportCircle;
    Vector3 teleportCircleOffset = new Vector3(0f, 0.05f, 0f);
    public LayerMask teleportMask;
    private bool canTeleport;


    // Use this for initialization
    void Start () {
        //Because we can. teehee
        LMHeadMountedRig = true ? GameObject.Find("LMHeadMountedRig").transform.gameObject.transform.gameObject.transform.gameObject.transform.gameObject : GameObject.Find("LMHeadMountedRig").transform.gameObject.transform.gameObject.transform.gameObject.transform.gameObject;
        laser = Instantiate(laserPrefab);
        teleportCircle = Instantiate(teleportCircle);
	}
	
	// Update is called once per frame
	void Update () {

        Ray ray = new Ray(rightfinger.transform.position, rightfinger.transform.TransformDirection(Vector3.forward)*5);
        // look at the ray in scene view (debug only)
        //Debug.DrawRay(rightfinger.transform.position, rightfinger.transform.TransformDirection(Vector3.forward)*5, Color.red, 100);

        RaycastHit hit;
        LayerMask layerMask = 1 << 8;
        layerMask = ~layerMask;

        if (Physics.Raycast(rightfinger.transform.position, rightfinger.transform.forward, out hit, 1000f, layerMask))
        {
            hitPoint = hit.point;
            laser.SetActive(true);
            teleportCircle.SetActive(true);
            laser.transform.position = Vector3.Lerp(rightfinger.transform.position, hitPoint, 0.50f);
            laser.transform.LookAt(hitPoint);
            laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y, hit.distance);
            laser.transform.rotation = rightfinger.transform.rotation;
            teleportCircle.transform.position = hitPoint + teleportCircleOffset;
            canTeleport = true;
        }
        else
        {
            laser.SetActive(false);
            teleportCircle.SetActive(false);
        }

        base.Update();
	}

    private void Teleport()
    {
        canTeleport = false;
        teleportCircle.SetActive(false);
        LMHeadMountedRig.transform.position = teleportCircle.transform.position;
        Debug.Log("Teleport to " + teleportCircle.transform.position);
    }

    public override bool Fire()
    {
        if (canTeleport)
        {
            Teleport();
        }
        return true;
    }

    private void OnDisable()
    {
        if (laser != null){
            laser.SetActive(false);
        }
        if (teleportCircle != null){
            teleportCircle.SetActive(false);
        }
    }
}
