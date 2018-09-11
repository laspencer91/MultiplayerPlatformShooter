using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerOnlyGameObject : MonoBehaviour
{
	void Start ()
    {
        if (GameSession.type == GameSessionType.offline)
        {
            Destroy(gameObject);
        }
	}
}
