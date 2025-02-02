using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    private GameManager gameManager;
    private CinemachineVirtualCamera vCam;
    private CinemachineInputProvider inputProvider;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        vCam = GetComponent<CinemachineVirtualCamera>();
        inputProvider = GetComponent<CinemachineInputProvider>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        Player.OnPlayerInstantiated += Player_OnPlayerSpawned;

        DisableCameraInputs();
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;

        Player.OnPlayerInstantiated -= Player_OnPlayerSpawned;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if(newState == GameState.PLAYING)
        {
            EnableCameraInputs();
        }
        else
        {
            DisableCameraInputs();
        }
    }

    private void Player_OnPlayerSpawned(Player spawnedPlayer)
    {
        Player.OnPlayerInstantiated -= Player_OnPlayerSpawned;

        AttachToTarget(spawnedPlayer.transform);
        EnableCameraInputs();
    }

    private void AttachToTarget(Transform targetTransform)
    {
        vCam.LookAt = targetTransform;
        vCam.Follow = targetTransform;
    }

    private void DisableCameraInputs()
    {
        inputProvider.enabled = false;
    }

    private void EnableCameraInputs()
    {
        inputProvider.enabled = true;
    }
}
