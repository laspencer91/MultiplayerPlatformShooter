using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    protected override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        //Debug.Log("Player is ready " + conn);
        //SpawnPlayer(conn);
    }
}
