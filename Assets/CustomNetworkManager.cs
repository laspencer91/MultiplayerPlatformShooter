using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    public static string PLAYER_ID_PREFIX = "Player ";

    ASpawnManager spawnManager;

    bool gameStarted = false;

    #region Initialize --------------------------------
    public override void OnStartHost()
    {
        base.OnStartHost();
        Initialize();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Initialize();
    }

    private void Initialize()
    {
        GameSession.networkManager = this;
        spawnManager = GetComponent<ASpawnManager>();
        if (spawnManager == null)
            spawnManager = gameObject.AddComponent<DefaultSpawnManager>();
    }
    #endregion

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Player " + conn.connectionId + " has been readied -----------------------------------------");
        GameSession.AddNetworkPlayerReady(conn, playerControllerId);
    }

    internal static string CreateID(Player player)
    {
        if (player.networkId == "")
            return PLAYER_ID_PREFIX + player.GetComponent<NetworkIdentity>().netId;
        else
            return player.networkId;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return) && !gameStarted)
        {
            gameStarted = true;
            GameSession.StartGame();
        }
    }

    /// <summary>
    /// Create a player using the given connection with the network managers player prefab
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    /// <returns></returns>
    public GameObject SpawnPlayerForConnection(NetworkConnection conn)
    {
        return spawnManager.SpawnPlayer(conn, playerPrefab);
    }
}
