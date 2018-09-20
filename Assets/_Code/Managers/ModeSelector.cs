using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelector : MonoBehaviour
{
    //[SerializeField] NetworkManager serverNetworkManagerPrefab;
    //[SerializeField] NetworkManager clientNetworkManagerPrefab;

    public void StartAsServer()
    {
        //ServerNetworkManager manager = (ServerNetworkManager) Instantiate(serverNetworkManagerPrefab);
        //manager.StartServer(3300, OnServerStartSuccess);
    }

    public void StartAsClient()
    {
        //ClientNetworkManager manager = (ClientNetworkManager) Instantiate(clientNetworkManagerPrefab);
        //manager.JoinServer("127.0.0.1", 3300, OnClientJoinSuccess);
    }

    public void OnClientJoinSuccess()
    {
        Debug.Log("Moving Scenes, client connected successfully?");
        GameSession.type = GameSessionType.online;
        SceneManager.LoadScene("Test Level");
    }

    public void OnServerStartSuccess()
    {
        Debug.Log("Moving Scenes, server started successfully?");
        GameSession.type = GameSessionType.online;
        SceneManager.LoadScene("Test Level");
    }
}
