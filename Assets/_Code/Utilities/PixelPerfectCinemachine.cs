using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPerfectCinemachine : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private CinemachineVirtualCamera virtualCameraScript;

    // Use this for initialization
    void Start()
    {
        virtualCameraScript = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        virtualCameraScript.m_Lens.OrthographicSize = mainCamera.orthographicSize;
    }
}
