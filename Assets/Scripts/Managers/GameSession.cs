using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : MonoBehaviour {

	void Awake ()
    {
        Singleton.AssertSingletonState<GameSession>(gameObject, true);
    }
	
	void Update ()
    {
		
	}
}
