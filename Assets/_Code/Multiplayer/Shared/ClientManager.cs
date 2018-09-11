using System;
using System.Collections.Generic;
using UnityEngine;

class ClientManager
{
    List<Client> clients = new List<Client>();

    public Client localClient { get; set; }

    public bool AddClient(Client newClient)
    {
        if (ClientExists(newClient.connectionId))
            return false;

        clients.Add(newClient);
        return true;
    }

    /// <summary>
    /// Create and add a new client based on connectionId. Will return false if
    /// the connectionId already exists. 
    /// </summary>
    /// <param name="connectionId">connectionId for new client</param>
    /// <returns>If client was successfully added</returns>
    public bool AddNewClient(int connectionId)
    {
        if (ClientExists(connectionId))
            return false;

        clients.Add(new Client((byte)connectionId));
        LogDebugMessage();
        return true;
    }

    public bool RemoveClient(Client clientToRemove)
    {
        return clients.Remove(clientToRemove);
    }

    public bool ClientExists(int id)
    {
        return GetClientById(id) != null;
    }

    public bool RemoveClient(int connectionId)
    {
        Client toRemove = GetClientById(connectionId);

        if (toRemove == null) return false;

        if (toRemove.pawn != null) GameObject.Destroy(toRemove.pawn.gameObject);

        return clients.Remove(toRemove);
    }

    public Client GetClientById(int id)
    {
        foreach (Client client in clients)
        {
            if (client.connectionId == id)
                return client;
        }
        Debug.Log("Client Not Found " + id);
        LogDebugMessage();
        return null;
    }

    public List<Client> GetClients()
    {
        return clients;
    }

    internal void AddClients(List<Client> clientList)
    {
        foreach (Client client in clientList)
        {
            AddClient(client);
        }
    }

    public void LogDebugMessage()
    {
        GameConsole.DebugLog("Debug Log:\n");
        foreach (Client client in clients)
        {
            GameConsole.DebugLog("ClientID: " + client.connectionId + "\n");
        }
    }

    internal void ClearAll()
    {
        clients.Clear();
    }
}
