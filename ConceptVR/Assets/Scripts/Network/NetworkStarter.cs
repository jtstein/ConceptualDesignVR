using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkStarter : MonoBehaviour {
    public bool startHost;
    private NetworkManager netManager;
    private string netAddress;
    private string netPort;
    private bool restart = false;
	// Use this for initialization
	void Start () {
        //Game needs to start off as networked for items to work and cannot be offline
        netManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        netManager.StartHost();
        restart = false;
        //For testing comment this out. Allows for faster testing of networking
        netManager.GetComponent<NetworkManagerHUD>().showGUI = false;

    }
	
	// Update is called once per frame
	void Update () {
        if (restart) return;
        //If anything happens to cause no network, restart the local host connection
        if (!netManager.isNetworkActive)
        {
            netManager.GetComponent<NetworkManagerHUD>().showGUI = false;
            netManager.networkAddress = "localhost";
            netManager.networkPort = 53535;

            netManager.StartHost();
        }
    }
    public void connectToHost()
    {
        //Allows connectivity to remote server.
        if (netAddress == null || netPort == null)
            return;
        netManager.StopHost();
        netManager.networkAddress = netAddress;
        netManager.networkPort = int.Parse(netPort);
        netManager.StartClient();
        netManager.GetComponent<NetworkManagerHUD>().showGUI = true;
        startHost = false;
        netAddress = null;
        netPort = null;
    }
    public void setNetAddress(string addr)
    {
        netAddress = addr;
    }
    public void setPort(string port)
    {
        netPort = port;
    }
    public void OnFailedToConnect()
    {
        Debug.Log("Failed to connect to Host");
        startHost = true;
    }
    //this is for a restart of the scene, scene will auto restart the network
    public void Restart()
    {
        restart = true;
        netManager.StopHost();
    }
}
