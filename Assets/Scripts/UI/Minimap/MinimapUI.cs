using Cinemachine;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    private PlayerControls playerControls; //minimap keybind
    private MinimapCameraController minimapCamera; //Top down camera that follows player (keeps them at center).

    [Header("References")]
    [SerializeField] private Transform background;
    [SerializeField] private Mask mask;
    [SerializeField] private RawImage rawImage;

    [Header("Config")]
    [SerializeField] private float maximizedCameraSize = 80f;
    [Range(0.0f, 1.0f)][SerializeField] private float maximizedOpacity = 0.75f;

    private Vector2 rawImageMinimizedSize;
    private Vector2 rawImageMinimizedPosition;
    private bool isMaximized = false; // maximized is when viewing full map

    private void Awake()
    {
        minimapCamera = FindObjectOfType<MinimapCameraController>();
        playerControls = FindObjectOfType<GameInputManager>().PlayerControls;

        playerControls.Gameplay.ToggleMinimap.performed += PlayerControls_OnToggleMinimapPerformed;

        rawImageMinimizedSize = rawImage.rectTransform.sizeDelta;
        rawImageMinimizedPosition = rawImage.rectTransform.localPosition;
    }

    private void OnEnable()
    {
        Maximize(false);
    }

    private void OnDestroy()
    {
        playerControls.Gameplay.ToggleMinimap.performed -= PlayerControls_OnToggleMinimapPerformed;
    }

    private void PlayerControls_OnToggleMinimapPerformed(InputAction.CallbackContext context)
    {
        if (isMaximized) Maximize(false);
        else Maximize(true);
    }

    private void Maximize(bool isMaximized)
    {
        this.isMaximized = isMaximized;

        background.gameObject.SetActive(!isMaximized);
        mask.enabled = !isMaximized;
        mask.GetComponent<Image>().enabled = !isMaximized;

        if (isMaximized)
        {
            Color transparentColor = rawImage.color;
            transparentColor.a = maximizedOpacity;
            rawImage.color = transparentColor;

            // Center Map and increase Size
            rawImage.rectTransform.sizeDelta = new Vector2(Camera.main.pixelHeight, Camera.main.pixelHeight);
            rawImage.rectTransform.position = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight)/2;

            minimapCamera.ChangeCameraSize(maximizedCameraSize);
            minimapCamera.EnableCameraBackground(false);
        }
        else
        {
            Color opaqueColor = rawImage.color;
            opaqueColor.a = 1f;
            rawImage.color = opaqueColor;

            // Make Minimap smaller and move into corner
            rawImage.rectTransform.sizeDelta = rawImageMinimizedSize;
            rawImage.rectTransform.localPosition = rawImageMinimizedPosition;

            minimapCamera.ResetCameraSize();
            minimapCamera.EnableCameraBackground(false);
        }
    }
}
