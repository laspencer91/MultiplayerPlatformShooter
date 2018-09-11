using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

class AllPlayerSpawnDataMessage : MessageBase
{
    public struct ClientSpawnData
    {
        public byte connectionId;
        public Vector3 position;
    }

    List<Client> clients;

    public List<ClientSpawnData> clientSpawnData;

    public AllPlayerSpawnDataMessage() { }

    public AllPlayerSpawnDataMessage(List<Client> clients)
    {
        this.clients = clients;
    }

    public override void Serialize(NetworkWriter writer)
    {
        byte numOfSpawns = 0;
        foreach (Client client in clients)
        {
            if (client.pawn != null)
                numOfSpawns += 1;
        }

        writer.Write(numOfSpawns);

        foreach (Client client in clients)
        {
            if (client.pawn != null)
            {
                writer.Write((byte) client.connectionId);
                writer.Write(client.pawn.transform.position);
            }
        }
    }

    public override void Deserialize(NetworkReader reader)
    {
        int numOfSpawns = reader.ReadByte();
        List<ClientSpawnData> clientSpawnData = new List<ClientSpawnData>();

        for (int i = 0; i < numOfSpawns; i++)
        {
            ClientSpawnData spawnData = new ClientSpawnData();
            spawnData.connectionId = reader.ReadByte();
            spawnData.position     = reader.ReadVector3();
            clientSpawnData.Add(spawnData);
        }

        this.clientSpawnData = clientSpawnData;
    }
}
