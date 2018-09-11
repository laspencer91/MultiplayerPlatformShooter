using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

class ServerOutgoingHandler
{
    NetworkManager m_Manager;

    public ServerOutgoingHandler(NetworkManager networkManager)
    {
        m_Manager = networkManager;
    }

    // Sends Initial Client Message With Information About The clients Id
    internal void SendInitialClientDataMessage(int connectionId)
    {
        List<Client> allClients = m_Manager.m_ClientManager.GetClients();
        ClientInitializationMessage message = new ClientInitializationMessage(connectionId, allClients);
        SendMessage(connectionId, m_Manager.reliableChannelId, NetMessageId.ClientInitialization, message);
    }

    internal void SendAllPlayerSpawnDataMessage(int connectionId)
    {
        List<Client> allClients = m_Manager.m_ClientManager.GetClients();
        AllPlayerSpawnDataMessage message = new AllPlayerSpawnDataMessage(allClients);
        SendMessage(connectionId, m_Manager.reliableChannelId, NetMessageId.AllPlayerSpawnData, message);
    }

    internal void SendClientConnectMessage(int connectionId)
    {
        SingleByteDataMessage message = new SingleByteDataMessage((byte)connectionId);
        SendMessageToAll(m_Manager.reliableChannelId, NetMessageId.ClientConnected, message);
    }

    internal void SendClientDisconnectMessage(int connectionId)
    {
        SingleByteDataMessage message = new SingleByteDataMessage((byte)connectionId);
        SendMessageToAll(m_Manager.reliableChannelId, NetMessageId.ClientDisconnected, message);
    }

    internal void SendPlayerSpawnMessage(byte connectionId, Vector3 position)
    {
        SendMessageToAll(m_Manager.reliableChannelId, NetMessageId.PlayerSpawn, new PlayerSpawnMessage(connectionId, position));
    }

    /// <summary>
    /// Sends a network message to a specific client
    /// </summary>
    /// <param name="connectionId">The client ID in which to send the message to</param>
    /// <param name="channel">The channel which to send the message on. Reliable or Unreliable</param>
    /// <param name="messageId">The Enum identifier of the message</param>
    /// <param name="message">The constructed message. Should be derived from MessageBase for serialization</param>
    protected void SendMessage(int connectionId, int channel, NetMessageId messageId, MessageBase message)
    {
        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage((short)messageId);
        message.Serialize(writer);
        writer.FinishMessage();

        byte[] buffer = writer.ToArray();
        byte error;

        NetworkTransport.Send(m_Manager.m_SocketId, connectionId, channel, buffer, buffer.Length, out error);
    }

    protected void SendMessageToAll(int channel, NetMessageId messageId, MessageBase message)
    {
        NetworkWriter writer = new NetworkWriter();
        writer.StartMessage((short)messageId);
        message.Serialize(writer);
        writer.FinishMessage();

        byte[] buffer = writer.ToArray();
        byte error;

        foreach (Client client in m_Manager.m_ClientManager.GetClients())
        {
            NetworkTransport.Send(m_Manager.m_SocketId, client.connectionId, channel, buffer, buffer.Length, out error);
        }
    }
}
