using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

class ClientNetworkManager : NetworkManager
{
    ClientStatus clientStatus = ClientStatus.Disconnected;

    public int m_ConnectionId { get; private set; }

    public Player localPlayerPrefab;

    Action onJoinSuccessCallback;

    ClientIncomingHandler m_IncomingHandler;

    protected override void Awake()
    {
        base.Awake();
        m_IncomingHandler = new ClientIncomingHandler(this);
    }

    public void JoinServer(string ip, int port, Action onSuccessCallback)
    {
        if (clientStatus == ClientStatus.Disconnected)
        {
            OpenSocketConnection(0);

            byte error;
            m_ConnectionId        = NetworkTransport.Connect(m_SocketId, ip, port, 0, out error);
            clientStatus          = ClientStatus.Connecting;
            onJoinSuccessCallback = onSuccessCallback;
        }
    }

    public override bool isServer()
    {
        return false;
    }

    public void SendPlayerInput(PlayerInput input)
    {
        SendMessage(unreliableChannelId, NetMessageId.PlayerInputUpdate, new PlayerInputUpdateMessage(input));
    }

    private void SendMessage(int channel, NetMessageId messageId, MessageBase message)
    {
        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage((short)messageId);
        message.Serialize(writer);
        writer.FinishMessage();

        byte[] buffer = writer.ToArray();
        byte error;

        NetworkTransport.Send(m_SocketId, m_ConnectionId, channel, buffer, buffer.Length, out error);
    }

    protected override void OnBroadcastEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error)
    {
        throw new NotImplementedException();
    }

    protected override void OnConnectEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error)
    {
        if (clientStatus != ClientStatus.Connecting) { Debug.Log("Connect Event but we arent connecting"); return; }

        if (m_ConnectionId == connectionId)
        {
            clientStatus = ClientStatus.Connected;
            onJoinSuccessCallback.Invoke();
            GameConsole.DebugLog("You have successfully connected to the server!");
        }
    }

    internal void OnDisconnect()
    {
        Debug.Log("We have been force disconnected");
        m_ClientManager.ClearAll();
        clientStatus = ClientStatus.Disconnected;
        SceneManager.LoadScene("Main Menu");
        Destroy(gameObject);
    }

    protected override void OnDataEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error)
    {
        NetworkReader reader   = new NetworkReader(recBuffer);
        short packetId         = reader.ReadInt16();               // Increases number each recieved packet?
        NetMessageId messageId = (NetMessageId) reader.ReadInt16();
        Debug.Log("Data Event With Message Id: " + messageId);

        m_IncomingHandler.RecieveMessage(reader, messageId);
    }

    internal void RemoveClient(byte dcedClientId)
    {
        GameConsole.DebugLog("Client " + dcedClientId + " has been disconnected");
        m_ClientManager.RemoveClient(dcedClientId);
    }

    protected override void OnDisconnectEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error)
    {
        Debug.Log("Disconnect Event Recieved");
        if (clientStatus == ClientStatus.Connecting && connectionId == m_ConnectionId)
        {
            Debug.Log("Connection to server was not successful");
            clientStatus = ClientStatus.Disconnected;
        }
        else if (clientStatus == ClientStatus.Connected)
        {
            Debug.Log("You have been disconnected from the server");
            OnDisconnect();
        }
    }

    protected override bool ShouldListenToNetwork()
    {
        return clientStatus != ClientStatus.Disconnected;
    }

    enum ClientStatus
    {
        Connected, Disconnected, Connecting 
    }
}
