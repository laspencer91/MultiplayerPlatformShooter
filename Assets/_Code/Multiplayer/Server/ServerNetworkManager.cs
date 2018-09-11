using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

class ServerNetworkManager : NetworkManager
{
    public Player playerPrefab;

    ServerState serverState = ServerState.stopped;

    public ServerIncomingHandler  m_IncomingHandler { get; private set; }
    public  ServerOutgoingHandler m_OutgoingHandler { get; private set; }
    private NetworkGameSession    m_GameSession;

    protected override void Awake()
    {
        base.Awake();
        m_OutgoingHandler = new ServerOutgoingHandler(this);
        m_IncomingHandler = new ServerIncomingHandler(this);
    }

    public void StartServer(int port, Action onServerStartSuccessCallback)
    {
        serverState = ServerState.running;
        OpenSocketConnection(port);
        onServerStartSuccessCallback.Invoke();

        SceneManager.sceneLoaded += OnSceneLoad;

        GameConsole.AddLine("Server Hosting Has Started");
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        m_GameSession = new TDMGameSession(m_ClientManager, this, true);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public void StopServer()
    {
        serverState = ServerState.stopped;
    }

    protected override void OnBroadcastEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error)
    {
        throw new NotImplementedException();
    }

    protected override void OnConnectEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error)
    {
        if (serverState == ServerState.stopped) { Debug.Log("Data recieved but server shutdown"); return; }

        bool added = m_ClientManager.AddNewClient(connectionId);

        if (!added)
        {
            Debug.Log("This connectionId is already connected to the server");
            return;
        }

        Debug.Log("New Client Connected With Connection Id: " + connectionId);
        m_OutgoingHandler.SendInitialClientDataMessage(connectionId);
        m_OutgoingHandler.SendClientConnectMessage(connectionId);
        m_OutgoingHandler.SendAllPlayerSpawnDataMessage(connectionId);
        m_GameSession.AssignPlayerTeam(m_ClientManager.GetClientById(connectionId));

        if (m_GameSession.allowWarmup)
        {
            m_GameSession.SpawnPlayer(m_ClientManager.GetClientById(connectionId));
        }
    }

    protected override void OnDataEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error)
    {
        NetworkReader reader = new NetworkReader(recBuffer);
        short packetId = reader.ReadInt16();               // Increases number each recieved packet?
        NetMessageId messageId = (NetMessageId)reader.ReadInt16();
        //Debug.Log("Data Event With Message Id: " + messageId);

        m_IncomingHandler.RecieveMessage(connectionId, reader, messageId);
    }

    protected override void OnDisconnectEvent(int connectionId, int channelId, byte[] recBuffer, int bufferSize, int dataSize, byte error)
    {
        if (!m_ClientManager.RemoveClient(connectionId))
        {
            Debug.LogWarning("Tried To Remove Client That Didnt Exists? Id: " + connectionId);
            return;
        }

        Debug.Log("Server Recieved Disconnect Event From Client: " + connectionId);
        m_ClientManager.RemoveClient(connectionId);
        m_OutgoingHandler.SendClientDisconnectMessage(connectionId);
    }

    protected override bool ShouldListenToNetwork()
    {
        return serverState == ServerState.running;
    }

    public override bool isServer()
    {
        return true;
    }

    enum ServerState { stopped, running }
}
