using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

class ServerIncomingHandler
{
    private ServerNetworkManager m_Manager;
    private ClientManager        m_ClientManager;

    public ServerIncomingHandler(ServerNetworkManager server)
    {
        m_Manager       = server;
        m_ClientManager = m_Manager.m_ClientManager;
    }

    public void RecieveMessage(int connectionId, NetworkReader reader, NetMessageId messageId)
    {
        switch (messageId)
        {
            case NetMessageId.PlayerInputUpdate:
                ReceivePlayerInputUpdate(connectionId, reader.ReadMessage<PlayerInputUpdateMessage>());
                break;
        }
    }

    private void ReceivePlayerInputUpdate(int connectionId, PlayerInputUpdateMessage playerInputUpdateMessage)
    {
        Player playerToUpdate = m_ClientManager.GetClientById(connectionId).pawn;

        if (playerToUpdate == null)
        {
            return;
        }

        playerToUpdate.GetComponent<LocalPlayerInputEngine>().SetInput(playerInputUpdateMessage.input);
    }
}
