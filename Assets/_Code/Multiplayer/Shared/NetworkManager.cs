using UnityEngine;
using UnityEngine.Networking;

abstract class NetworkManager : MonoBehaviour
{
    [SerializeField] protected static int MAX_PLAYERS = 10;

    protected HostTopology  topology;
    public    ClientManager m_ClientManager { get; protected set; }

    public int reliableChannelId   { get; protected set; }
    public int unreliableChannelId { get; protected set; }
    public int m_SocketId = -1;

    protected virtual void Awake()
    {
        Singleton.AssertSingletonState<NetworkManager>(gameObject, true);
        m_ClientManager = new ClientManager();
        NetworkTransport.Init();
        CreateNetTopology();
    }

    private void CreateNetTopology()
    {
        ConnectionConfig config = new ConnectionConfig();
        reliableChannelId       = config.AddChannel(QosType.Reliable);
        unreliableChannelId     = config.AddChannel(QosType.Unreliable);

        if (isServer())
            topology = new HostTopology(config, MAX_PLAYERS);
        else
            topology = new HostTopology(config, 1);
    }

    protected void OpenSocketConnection(int port)
    {
        if (m_SocketId >= 0) return;
        m_SocketId = NetworkTransport.AddHost(topology, port);
    }

    void Update()
    {
        if (ShouldListenToNetwork())
            NetworkListen();
    }

    int recConnectionId, channelId, dataSize, bufferSize = 1024;

    protected virtual void NetworkListen()
    {
        byte[] recBuffer = new byte[1024];
        byte error;

        NetworkEventType recData = NetworkTransport.ReceiveFromHost(m_SocketId, out recConnectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing: break;
                case NetworkEventType.ConnectEvent:
                    OnConnectEvent(recConnectionId, channelId, recBuffer, bufferSize, dataSize, error); break;
                case NetworkEventType.DataEvent:
                    OnDataEvent(recConnectionId, channelId, recBuffer, bufferSize, dataSize, error); break;
                case NetworkEventType.DisconnectEvent:
                    OnDisconnectEvent(recConnectionId, channelId, recBuffer, bufferSize, dataSize, error); break;
                case NetworkEventType.BroadcastEvent:
                    OnBroadcastEvent(recConnectionId, channelId, recBuffer, bufferSize, dataSize, error); break;
            }

            recData = NetworkTransport.ReceiveFromHost(m_SocketId, out recConnectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        }
    }

    protected abstract bool ShouldListenToNetwork();

    protected abstract void OnConnectEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error);
    protected abstract void OnDataEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error);
    protected abstract void OnDisconnectEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error);
    protected abstract void OnBroadcastEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error);

    public abstract bool isServer();
}
