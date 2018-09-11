using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class TDMGameSession : NetworkGameSession
{
    public static int MinPlayers = 1;

    public int[] teamScores = new int[2];

    public int winningScore = 50;

    private List<Client>[] teams = new List<Client>[2];

    public TDMGameSession(ClientManager clientManager, ServerNetworkManager server, bool allowWarmup) 
        : base(clientManager, server, allowWarmup)
    {
        for (int i = 0; i < teams.Length; i++)
            teams[i] = new List<Client>();

        m_SpawnManager = UnityEngine.Object.FindObjectOfType<SpawnManager>();
    }

    public override bool ReadyToStart()
    {
        return NumberConnectedClients() >= MinPlayers;
    }

    public override void SpawnPlayer(Client clientToSpawn)
    {
        if (clientToSpawn.pawn != null) return;

        Team playersTeam = GetPlayersAssignedTeam(clientToSpawn);
        if (playersTeam == Team.None) return;

        SpawnPoint playerSpawn   = m_SpawnManager.GetRandomSpawnPoint(playersTeam);
        Player     spawnedPlayer = UnityEngine.Object.Instantiate(m_Server.playerPrefab, playerSpawn.transform.position, Quaternion.identity);
        clientToSpawn.pawn       = spawnedPlayer;

        clientToSpawn.pawn.isLocalPlayer = false;
        m_Server.m_OutgoingHandler.SendPlayerSpawnMessage((byte)clientToSpawn.connectionId, playerSpawn.transform.position);
    }

    public override void AssignPlayerTeam(Client clientToGiveTeam)
    {
        if (clientToGiveTeam == null) { Debug.Log("Client given to AssignPlayerTeam was null"); return; }
        if (PlayerHasAssignedTeam(clientToGiveTeam)) return;

        Team playersTeam = (teams[(int)Team.Red].Count < teams[(int)Team.Green].Count) ? Team.Red : Team.Green;

        teams[(int)playersTeam].Add(clientToGiveTeam);
        // Todo Send Team Message Over Network
    }

    public void AddTeamPoint(Team scoringTeam, int points)
    {
        teamScores[(int)scoringTeam] += points;
    }

    public override bool WinConditionMet()
    {
        return teamScores[0] >= winningScore || teamScores[1] >= winningScore;
    }

    public override WinData GetWinData()
    {
        if (!WinConditionMet()) return new WinData(Team.None, -1);

        // A tie
        if (teamScores[0] == teamScores[1] && teamScores[0] >= winningScore)
        {
            return new WinData(Team.All, teamScores[0]);   // Todo need to resolve a tie
        }
        // Green Team Win
        else if (teamScores[(int)Team.Green] >= winningScore)
        {
            return new WinData(Team.Green, teamScores[(int)Team.Green]);   // Todo need to resolve a tie
        }
        else
        {
            return new WinData(Team.Red, teamScores[(int)Team.Red]);   // Todo need to resolve a tie
        }
    }

    private Team GetPlayersAssignedTeam(Client client)
    {
        for (int i = 0; i < teams.Length; i++)
        {
            if (teams[i].Contains(client)) return (Team) i;
        }
        return Team.None;
    }

    private bool PlayerHasAssignedTeam(Client client)
    {
        foreach (List<Client> team in teams)
        {
            if (team.Contains(client)) return true;
        }
        return false;
    }
}
