using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSessionType type = GameSessionType.offline;

	void Awake ()
    {
        Singleton.AssertSingletonState<GameSession>(gameObject, true);
    }
}

public enum GameSessionType { offline, online }
