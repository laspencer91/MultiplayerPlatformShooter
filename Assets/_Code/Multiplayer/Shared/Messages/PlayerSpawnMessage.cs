using UnityEngine;
using UnityEngine.Networking;

class PlayerSpawnMessage : MessageBase
{
    public byte clientId;
    public Vector3 position;

    public PlayerSpawnMessage() { }

    public PlayerSpawnMessage(byte connectionId, Vector3 spawnPosition)
    {
        clientId = connectionId;
        position = spawnPosition;
    }
}
