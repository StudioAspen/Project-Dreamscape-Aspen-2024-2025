using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CameraFocusType
{
    Launch,
    Slam,
}

public class CameraManager : MonoBehaviour
{

    public static CameraManager Instance;

    public CinemachineVirtualCamera GameplayCamera;
    public CinemachineVirtualCamera LaunchCinematicCamera;
    public CinemachineVirtualCamera SlamCinematicCamera;

    private void Awake()
    {
        Instance = this;
    }

    public void SwitchToCinematicCamera(CameraFocusType focusType)
    {
        switch (focusType)
        {
            case CameraFocusType.Launch:
                LaunchCinematicCamera.Priority = 10;
                SlamCinematicCamera.Priority = 0;
                GameplayCamera.Priority = 0;
                break;

            case CameraFocusType.Slam:
                SlamCinematicCamera.Priority = 10;
                LaunchCinematicCamera.Priority = 0;
                GameplayCamera.Priority = 0;
                break;
        }
    }

    public void SwitchToGameplayCamera()
    {
        GameplayCamera.Priority = 10;
        LaunchCinematicCamera.Priority = 0;
        SlamCinematicCamera.Priority = 0;
    }
}
