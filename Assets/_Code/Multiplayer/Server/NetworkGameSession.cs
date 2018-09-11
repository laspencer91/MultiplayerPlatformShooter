struct WinData
{
    Team winningTeam;
    int  winningScore;

    public WinData(Team winningTeam, int winningScore)
    {
        this.winningScore = winningScore;
        this.winningTeam  = winningTeam;
    }
}

public enum SessionStatus { Warmup, Active }

abstract class NetworkGameSession
{
    protected ServerNetworkManager m_Server;
    protected ClientManager m_ClientManager;

    public SpawnManager m_SpawnManager { get; set; }
    public bool allowWarmup { get; private set; }

    public SessionStatus status { get; protected set; }

    public NetworkGameSession(ClientManager clientManager, ServerNetworkManager server, bool allowWarmup)
    {
        m_ClientManager = clientManager;
        m_Server = server;
        this.allowWarmup = allowWarmup;
        this.status      = SessionStatus.Warmup;
    }

    protected int NumberConnectedClients()
    {
        return m_ClientManager.GetClients().Count;
    }

    public abstract void AssignPlayerTeam(Client clientToGiveTeam);

    public abstract void SpawnPlayer(Client clientToSpawn);

    public abstract bool ReadyToStart();

    public abstract bool WinConditionMet();

    public abstract WinData GetWinData();
}