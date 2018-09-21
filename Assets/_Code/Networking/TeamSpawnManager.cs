using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TeamSpawnManager : ASpawnManager
{
    private SpawnPoint[] allSpawnPoints;

    private Dictionary<Team, List<SpawnPoint>> teamSpawnPoints;

    private List<SpawnPoint> usedSpawnPoints;

    private void Awake()
    {
        allSpawnPoints  = FindObjectsOfType<SpawnPoint>();
        teamSpawnPoints = new Dictionary<Team, List<SpawnPoint>>();
        usedSpawnPoints = new List<SpawnPoint>();

        InitializeSpawnPointDataStructure();
    }

    private void InitializeSpawnPointDataStructure()
    {
        // Create a list for each team
        for (int i = 0; i < (int)Team.Count; i++)
        {
            teamSpawnPoints.Add((Team)i, new List<SpawnPoint>());
        }

        // Add all the spawn points to their respected teams
        foreach (SpawnPoint spawn in allSpawnPoints)
        {
            teamSpawnPoints[spawn.team].Add(spawn);
        }
    }

    public SpawnPoint GetSpawnPointAutoFill()
    {
        int[] spawnCounts = new int[(int)Team.Count];

        foreach (SpawnPoint point in usedSpawnPoints)
        {
            spawnCounts[(int)point.team]++;
        }
        int smallestTeamCount = spawnCounts.Min();

        for (int i = 0; i < spawnCounts.Length; i++)
            if (spawnCounts[i] == smallestTeamCount)
                return GetRandomSpawnPoint((Team)i);

        return null;
    }

    public SpawnPoint GetRandomSpawnPoint(Team team)
    {
        if (team == Team.None || team == Team.All || team == Team.Count)
        {
            return null;
        }

        List<SpawnPoint> list = teamSpawnPoints[team];
        for (int i = 0; i < list.Count; i++)
        {
            if (!usedSpawnPoints.Contains(list[i]))
            {
                usedSpawnPoints.Add(list[i]);
                return list[i];
            }
        }
        return list[0];
    }

    public SpawnPoint GetSpawnPoint(Team team, int index)
    {
        List<SpawnPoint> list = teamSpawnPoints[team];
        usedSpawnPoints.Add(list[index]);
        return list[index];
    }

    public void ResetUsedSpawnPoints()
    {
        usedSpawnPoints.Clear();
    }

    public override GameObject SpawnPlayer(NetworkConnection conn, GameObject playerPrefab)
    {
        GameObject           player;
        SpawnPoint           spawnPoint = GetSpawnPointAutoFill();
        Transform            startPos   = (spawnPoint != null) ? spawnPoint.transform : null;

        if (startPos != null)
        {
            player = GameObject.Instantiate(playerPrefab, startPos.position, startPos.rotation);
        }
        else
        {
            player = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }

        return player;
    }
}
