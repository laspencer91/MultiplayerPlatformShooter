using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSaver : MonoBehaviour
{
    Vector3 startPosition;

	void Awake ()
    {
        startPosition = transform.localPosition;
	}

    private void OnEnable()
    {
        transform.localPosition = startPosition;
    }

    private void OnDisable()
    {
        transform.localPosition = startPosition;
    }
}
