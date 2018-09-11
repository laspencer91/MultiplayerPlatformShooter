using System.Collections.Generic;
using UnityEngine.Networking;

class ClientInitializationMessage : MessageBase
{
    // List of clients contained by the server
    public List<Client> clients;

    // The connectionID from the servers perspective of the reciever 
    public byte recieversConnectionId;

    public ClientInitializationMessage() { }

    public ClientInitializationMessage(int connectionId, List<Client> clients)
    {
        recieversConnectionId = (byte) connectionId;
        this.clients          = clients;
    }

    public override void Serialize(NetworkWriter writer)
    {
        writer.Write(recieversConnectionId);        // Write the id of the reciever
        NetworkUtils.WriteByteCountAndList(clients, writer);
    }

    public override void Deserialize(NetworkReader reader)
    {
        recieversConnectionId = reader.ReadByte();
        clients = NetworkUtils.ReadByteCountAndList<Client>(reader);
    }
}
