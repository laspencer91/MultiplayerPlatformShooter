using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private SpawnPoint[] allSpawnPoints;

    private Dictionary<Team, List<SpawnPoint>> teamSpawnPoints = new Dictionary<Team, List<SpawnPoint>>();

    private List<SpawnPoint> usedSpawnPoints = new List<SpawnPoint>();

    void Awake ()
    {
        allSpawnPoints = FindObjectsOfType<SpawnPoint>();
        InitializeSpawnPointDataStructure();
    }

    private void InitializeSpawnPointDataStructure()
    {
        // Create a list for each team
        for (int i = 0; i < (int) Team.Count; i++)
        {
            teamSpawnPoints.Add((Team)i, new List<SpawnPoint>());
        }

        // Add all the spawn points to their respected teams
        foreach (SpawnPoint spawn in allSpawnPoints)
        {
            teamSpawnPoints[spawn.team].Add(spawn);
        }
    }

	public SpawnPoint GetRandomSpawnPoint(Team team)
    {
        if (team == Team.None || team == Team.All)
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
}
