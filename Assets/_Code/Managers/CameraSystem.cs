using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    CinemachineVirtualCamera vCam;

	void Awake()
    {
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        if (vCam == null) Debug.LogWarning("Cinemachine Virtual Camera Not Found In Camera System Children");
	}
	
	public void SetCameraTarget(GameObject target)
    {
        vCam.Follow = target.transform;
    }
}
