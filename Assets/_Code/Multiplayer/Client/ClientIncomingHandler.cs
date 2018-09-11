using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

class ClientIncomingHandler
{
    private ClientNetworkManager m_Manager;
    private ClientManager        m_ClientManager;

    public ClientIncomingHandler(ClientNetworkManager clientNetworkManager)
    {
        m_Manager       = clientNetworkManager;
        m_ClientManager = m_Manager.m_ClientManager;
    }

    public void RecieveMessage(NetworkReader reader, NetMessageId messageId)
    {
        switch (messageId)
        {
            case NetMessageId.ClientInitialization:
                ReceiveInitializationMessage(reader.ReadMessage<ClientInitializationMessage>());
                break;
            case NetMessageId.ClientDisconnected:
                ReceiveClientDisconnectedMessage(reader.ReadMessage<SingleByteDataMessage>());
                break;
            case NetMessageId.ClientConnected:
                ReceiveClientConnectedMessage(reader.ReadMessage<SingleByteDataMessage>());
                break;
            case NetMessageId.PlayerSpawn:
                ReceivePlayerSpawnMessage(reader.ReadMessage<PlayerSpawnMessage>());
                break;
            case NetMessageId.AllPlayerSpawnData:
                ReceiveAllPlayerSpawnDataMessage(reader.ReadMessage<AllPlayerSpawnDataMessage>());
                break;
        }
    }

    public void ReceiveClientConnectedMessage(SingleByteDataMessage clientConnectedMessage)
    {
        if (clientConnectedMessage.data == m_ClientManager.localClient.connectionId) return;

        m_ClientManager.AddNewClient(clientConnectedMessage.data);
        Debug.Log("New Player Joined With Id: " + clientConnectedMessage);
    }

    private void ReceivePlayerSpawnMessage(PlayerSpawnMessage playerSpawnMessage)
    {
        if (playerSpawnMessage.clientId == m_ClientManager.localClient.connectionId)
        {
            GameConsole.DebugLog("Client Id Is My Local Id, Creating My Player: " + playerSpawnMessage.clientId);
            Player myPlayer = GameObject.Instantiate(m_Manager.localPlayerPrefab, playerSpawnMessage.position, Quaternion.identity);
            m_ClientManager.localClient.pawn = myPlayer;
        }
        else
        {   // TODO Instantiate dummy player object?
            GameConsole.DebugLog("Client Id Is My Other Id, Creating Their Player: " + playerSpawnMessage.clientId);
            Player otherPlayer = GameObject.Instantiate(m_Manager.localPlayerPrefab, playerSpawnMessage.position, Quaternion.identity);
            Client otherClient = m_ClientManager.GetClientById(playerSpawnMessage.clientId);

            if (otherClient == null) GameConsole.DebugLog("Something is wrong: " + playerSpawnMessage.clientId);

            otherClient.pawn   = otherPlayer;
            otherPlayer.isLocalPlayer = false;
        }
    }

    private void ReceiveInitializationMessage(ClientInitializationMessage message)
    {
        Client myLocalClient = new Client(message.recieversConnectionId);
        bool   addSuccess    = m_ClientManager.AddClient(myLocalClient);

        if (!addSuccess)
            Debug.LogWarning("Client already existed when trying to add. Add did not take place");
        else
            m_ClientManager.localClient = myLocalClient;

        m_ClientManager.AddClients(message.clients);
        m_ClientManager.LogDebugMessage();
    }

    private void ReceiveAllPlayerSpawnDataMessage(AllPlayerSpawnDataMessage message)
    {
        List<AllPlayerSpawnDataMessage.ClientSpawnData> spawnData = message.clientSpawnData;

        for (int i = 0; i < spawnData.Count; i++)
        {
            Client cl = m_ClientManager.GetClientById(spawnData[i].connectionId);
            if (cl == null) { GameConsole.DebugLog("Player trying to spawn but not shown to be connected, ID: " + spawnData[i].connectionId); continue; }

            Player pl = GameObject.Instantiate(m_Manager.localPlayerPrefab, spawnData[i].position, Quaternion.identity);
            pl.isLocalPlayer = false;
            cl.pawn = pl;
        }
    }

    private void ReceiveClientDisconnectedMessage(SingleByteDataMessage singleByteDataMessage)
    {
        byte dcedClientId = singleByteDataMessage.data;
        if (dcedClientId == m_ClientManager.localClient.connectionId)
        {
            m_Manager.OnDisconnect(); // Disconnect Us
        }
        else
        {
            Client client = m_ClientManager.GetClientById(dcedClientId);

            if (client != null)
                if (client.pawn != null)
                    GameObject.Destroy(client.pawn.gameObject);

            m_Manager.RemoveClient(dcedClientId);   // Disconnect Other Client
        }
    }
}
