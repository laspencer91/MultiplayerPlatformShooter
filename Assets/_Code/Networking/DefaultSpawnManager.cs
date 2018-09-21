using Mirror;
using System;
using UnityEngine;

[Serializable]
public class DefaultSpawnManager : ASpawnManager
{
    public override GameObject SpawnPlayer(NetworkConnection conn, GameObject playerPrefab)
    {
        GameObject player;
        Transform startPos = GameObject.FindObjectOfType<NetworkStartPosition>().transform;

        if (startPos != null)
        {
            player = GameObject.Instantiate(playerPrefab, startPos.position, startPos.rotation);
        }
        else
        {
            player = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }

        return player;
    }
}
