using Cinemachine;
using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Self] private CinemachineVirtualCamera vCam;
    [SerializeField, Self] private CinemachineInputProvider inputProvider;
    [SerializeField, Scene] private Player player;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
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
