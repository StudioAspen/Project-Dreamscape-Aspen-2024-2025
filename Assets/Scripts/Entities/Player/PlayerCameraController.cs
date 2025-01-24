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
    private Player player;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        vCam = GetComponent<CinemachineVirtualCamera>();
        inputProvider = GetComponent<CinemachineInputProvider>();
        player = FindObjectOfType<Player>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        AttachToPlayer();
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

    private void AttachToPlayer()
    {
        vCam.LookAt = player.transform;
        vCam.Follow = player.transform;
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
