using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession
{
    public static GameSessionType                type          = GameSessionType.offline;
    public static GameSessionStatus              status        = GameSessionStatus.Pregame;
    public static Dictionary<string, PlayerData> activePlayers = new Dictionary<string, PlayerData>();
    public static CustomNetworkManager           networkManager;

    static List<InitialData> pregamePlayerData = new List<InitialData>();

    public static void AddNetworkPlayerReady(NetworkConnection conn, short controllerId)
    {
        Debug.Log("Player added to preGame: " + conn);
        pregamePlayerData.Add(new InitialData(conn, controllerId));
    }

    public static void StartGame()
    {
        Debug.Log("Start game called: ");
        foreach (InitialData data in pregamePlayerData)
        {
            GameObject p_instance = networkManager.SpawnPlayerForConnection(data.conn);
            NetworkServer.AddPlayerForConnection(data.conn, p_instance, data.controllerId);

            Player createdPlayer = p_instance.GetComponent<Player>();
            RegisterPlayer(createdPlayer, data.conn);
        }
        pregamePlayerData.Clear();
        status = GameSessionStatus.Active;

        // Debug the registration
        foreach (string playerId in activePlayers.Keys)
        {
            Debug.Log(activePlayers[playerId].networkId + " : " + activePlayers[playerId].connection.connectionId + " is successfully registered for game start.");
        }
    }

    /// <summary>
    /// Register a player as active in the game
    /// </summary>
    /// <param name="player">The instantiated Player component</param>
    /// <param name="conn">The network connection associated with the instance</param>
    public static void RegisterPlayer(Player player, NetworkConnection conn)
    {
        if (player.networkId == "")
        {
            // Assign a player id if its not already assigned
            Debug.LogWarning("NetworkId of Player On Register Is Empty. Setting the Id Here");
            player.networkId = CustomNetworkManager.CreateID(player);
        }

        activePlayers.Add(player.networkId, new PlayerData(player.networkId, conn, player.gameObject));
        Debug.Log("Player successfully added to active players: " + player.networkId);
    }

    public static void UnregisterPlayer(Player player)
    {
        if (activePlayers.ContainsKey(player.networkId))
        {
            activePlayers.Remove(player.networkId);
            Debug.Log("Player has been removed from active players: " + player.networkId);
        }
    }

    /// <summary>
    /// Struct for holding initial data. TODO Convert to Tuple with new C#
    /// </summary>
    struct InitialData
    {
        public NetworkConnection conn; public short controllerId;

        public InitialData(NetworkConnection conn, short controllerId)
        {
            this.conn = conn;
            this.controllerId = controllerId;
        }
    }
}

public enum GameSessionType   { offline, online }
public enum GameSessionStatus { Pregame, Active, PostGame}

public struct PlayerData
{
    public string networkId;
    public NetworkConnection connection;
    public GameObject pawn;

    public PlayerData(string networkId, NetworkConnection connection, GameObject pawn)
    {
        this.networkId = networkId;
        this.connection = connection;
        this.pawn = pawn;
    }
}