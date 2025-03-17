using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    private InputManager inputManager;
    private GameManager gameManager;
    private CinemachineVirtualCamera vCam;
    private CinemachineInputProvider inputProvider;

    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        inputProvider = GetComponent<CinemachineInputProvider>();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        inputManager = FindAnyObjectByType<InputManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        DisableCameraInputs();

        Player player = FindObjectOfType<Player>(); // tries to find player first
        if(player != null) AttachToTarget(player.transform);
        Player.OnPlayerLoaded += Player_OnPlayerLoaded; // If player doesnt exist yet, wait for it to be loaded

        PlayerPreferences.Instance.OnCameraSensitivityChanged += SetCameraSensitivity;

        GameManager_OnGameStateChanged(gameManager.CurrentState); // Manually call this to set the initial state of the camera
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        Player.OnPlayerLoaded -= Player_OnPlayerLoaded;
        PlayerPreferences.Instance.OnCameraSensitivityChanged -= SetCameraSensitivity;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.PLAYING)
        {
            EnableCameraInputs();
        }
        else
        {
            DisableCameraInputs();
        }
    }

    private void Player_OnPlayerLoaded(Player player)
    {
        AttachToTarget(player.transform);

        Player.OnPlayerLoaded -= Player_OnPlayerLoaded;
    }

    private void AttachToTarget(Transform targetTransform)
    {
        vCam.LookAt = targetTransform;
        vCam.Follow = targetTransform;
    }

    private void EnableCameraInputs()
    {
        inputProvider.enabled = true;
    }

    private void DisableCameraInputs()
    {
        inputProvider.enabled = false;
    }

    private void SetCameraSensitivity(float sensitivity) {
        CinemachinePOV pov = vCam.GetCinemachineComponent<CinemachinePOV>();
        if (pov != null) 
        {
            pov.m_HorizontalAxis.m_MaxSpeed = sensitivity;
            pov.m_VerticalAxis.m_MaxSpeed = sensitivity;
        }
    }
    
}
