using UnityEngine;
using UnityEngine.Networking;

class Client : INetworkSerializable
{
    public int connectionId { get; private set; } // Client Id                     

    public Player pawn { get; set; }

    public Client(int connectionId)
    {
        this.connectionId = connectionId;
    }
    
    public Client()
    { /* Need empty constructor for deserialize to work*/ }

    #region Network Serialization
        public void Serialize(NetworkWriter writer)
        {
            writer.Write((byte) connectionId);
        }

        public void Deserialize(NetworkReader reader)
        {
            connectionId = reader.ReadByte();
        }
    #endregion
}
