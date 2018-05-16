using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetServer : NetworkManager {
    private NetworkStarter netStarter;
    public void Start() { 
        netStarter = GameObject.Find("NetworkStarter").GetComponent<NetworkStarter>();
        if(playerPrefab == null)
        {
            playerPrefab = GameObject.Find("PlayerPrefab");
        }
        this.playerPrefab.SetActive(true);
    }
    public override void OnStartHost()
    {
        Debug.Log("Hosting match at: " + this.networkAddress + ":" + this.networkPort);
        base.OnStartHost();
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (playerPrefab == null)
        {
            playerPrefab = GameObject.Find("PlayerPrefab");
        }
        Debug.Log("Player connected to server with id of: " + playerControllerId);
        base.OnServerAddPlayer(conn, playerControllerId);
    }
    public override void OnServerError(NetworkConnection conn, int errorCode)
    {
        Debug.Log("Server Error: " + conn.lastError + " ErrorCode: " + errorCode);
        base.OnServerError(conn, errorCode);
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NetworkServer.DestroyPlayersForConnection(conn);
        if (conn.lastError != NetworkError.Ok)
        {
            if (LogFilter.logError)
            {
                Debug.LogError("ServerDisconnected due to error: " + conn.lastError);
            }
        }
        base.OnServerDisconnect(conn);
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("I have connected with the ID of: " + conn.connectionId);
        base.OnClientConnect(conn);
    }
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log(conn.lastError);
        base.OnClientDisconnect(conn);
    }
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        Debug.Log(conn.lastError + ":" + errorCode);
        base.OnClientError(conn, errorCode);
    }
}
