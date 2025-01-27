using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChanger : MonoBehaviour
{
    // Cameras
    [SerializeField] CinemachineVirtualCamera wallJumpCamera;
    [SerializeField] CinemachineFreeLook mainCamera;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wallJumpCamera.Priority = mainCamera.Priority + 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wallJumpCamera.Priority = mainCamera.Priority - 1;
        }
    }
}
