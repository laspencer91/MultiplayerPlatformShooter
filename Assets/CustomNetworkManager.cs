using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerReady(NetworkConnection conn)
    {
        Debug.Log("Player is ready " + conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Player Added. Spawn?");
    }
}
