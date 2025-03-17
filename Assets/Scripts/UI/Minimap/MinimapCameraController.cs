using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MinimapCameraController : MonoBehaviour
{
    private Camera selfCamera;
    private UniversalAdditionalCameraData universalAdditionalCameraData;
    private Player player;

    [Header("Config")]
    [SerializeField] private float cameraHeight = 15f;
    [SerializeField] private float cameraSize = 16f;
    private float originalCameraSize;

    private void Awake()
    {
        selfCamera = GetComponent<Camera>();
        universalAdditionalCameraData = GetComponent<UniversalAdditionalCameraData>();

        originalCameraSize = cameraSize;
    }

    private void Start()
    {
        player = FindObjectOfType<Player>(); // Tries to find player first
        Player.OnPlayerLoaded += Player_OnPlayerLoaded; // For when it doesnt exist
    }

    private void OnDestroy()
    {
        Player.OnPlayerLoaded -= Player_OnPlayerLoaded;
    }

    private void Player_OnPlayerLoaded(Player player)
    {
        Player.OnPlayerLoaded -= Player_OnPlayerLoaded;
        this.player = player;
    }

    private void Update()
    {
        if (player == null) return;

        transform.position = new Vector3(player.transform.position.x, cameraHeight, player.transform.position.z);
        selfCamera.orthographicSize = cameraSize;
    }

    public void EnableCameraBackground(bool enable)
    {
        universalAdditionalCameraData.renderPostProcessing = enable;
    }

    public void ChangeCameraSize(float newSize)
    {
        cameraSize = newSize;
    }

    public void ResetCameraSize()
    {
        cameraSize = originalCameraSize;
    }
}
