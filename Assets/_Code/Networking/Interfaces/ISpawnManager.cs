using Mirror;
using System;
using UnityEngine;

public abstract class ASpawnManager : MonoBehaviour, ISpawnManager
{
    /// <summary>
    /// Called by the server when it is time to spawn a player for a connection. This method
    /// should instantiate a new instance somewhere and then return that instance to be used
    /// for the given connection.
    /// </summary>
    /// <param name="conn">The connection for the instance to be associated with.</param>
    /// <param name="playerPrefab">The prefab to use for instance instantiation</param>
    /// <returns>The instantiated Object</returns>
    public abstract GameObject SpawnPlayer(NetworkConnection conn, GameObject playerPrefab);
}

public interface ISpawnManager
{
    /// <summary>
    /// Called by the server when it is time to spawn a player for a connection. This method
    /// should instantiate a new instance somewhere and then return that instance to be used
    /// for the given connection.
    /// </summary>
    /// <param name="conn">The connection for the instance to be associated with.</param>
    /// <param name="playerPrefab">The prefab to use for instance instantiation</param>
    /// <returns>The instantiated Object</returns>
    GameObject SpawnPlayer(NetworkConnection conn, GameObject playerPrefab);
}