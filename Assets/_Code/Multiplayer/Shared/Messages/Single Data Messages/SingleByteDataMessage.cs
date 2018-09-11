using UnityEngine.Networking;

class SingleByteDataMessage : MessageBase
{
    public byte data;

    public SingleByteDataMessage(byte data)
    {
        this.data = data;
    }

    public SingleByteDataMessage() { }
}
