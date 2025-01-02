using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.InputSystem.LowLevel;

public class MinimapController : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private RectTransform minimapRectTransform; // Raw Image Render Texture
    private Vector2 normalSize;
    private Vector2 normalPosition;
    private Vector2 centeredPosition = new Vector2(0, 0);
    private bool isMaximized = false;
    private float originalZoom;

    [SerializeField] private Vector2 maximizedSize = new Vector2(300, 300);
    [SerializeField] private Camera m_Camera;
    [SerializeField] private int maximizedZoom = 80;
    [Range(0.0f, 1.0f)][SerializeField] private float minimap_opacity = 0.75f;

    private CinemachineVirtualCamera virtualCamera;
    private Cinemachine3rdPersonFollow thirdPersonFollow;
    private Mask mask;
    private RawImage image;
    private Transform border;
    [SerializeField] private Canvas minimap_canvas;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        // Cache Components
        normalSize = minimapRectTransform.sizeDelta;
        normalPosition = minimapRectTransform.anchoredPosition;
        originalZoom = m_Camera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>().VerticalArmLength;

        RectTransform canvasRT = GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
        centeredPosition = new Vector2(-(canvasRT.rect.width / 2), canvasRT.rect.height / 2);

        mask = GetComponentInChildren<Mask>();
        virtualCamera = m_Camera.GetComponent<CinemachineVirtualCamera>();
        thirdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        image = mask.GetComponentInChildren<RawImage>();
        border = transform.GetChild(0).Find("Border");
    }
    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    public void ToggleMinimap()
    {
        // Disable the Toggle when the game isn't running
        if ((gameManager.CurrentState != GameState.PLAYING) && !isMaximized)
        {
            return;
        }

        // Toggle Mask and Border
        if (border != null)
        {
            border.gameObject.SetActive(isMaximized);
        }

        if (mask != null)
        {
            mask.enabled = !mask.enabled;
            mask.GetComponent<Image>().enabled = !mask.GetComponent<Image>().enabled;
        }

        // Toggle Solid Color Background of Minimap
        if (m_Camera.TryGetComponent(out UniversalAdditionalCameraData cameraData))
        {
            cameraData.renderPostProcessing = !cameraData.renderPostProcessing;
        }

        // Map is Maximized --> Minimize map
        if (isMaximized)
        {
            // Make Minimap smaller and move into corner
            minimapRectTransform.sizeDelta = normalSize;
            minimapRectTransform.anchoredPosition = normalPosition;
            thirdPersonFollow.VerticalArmLength = originalZoom;

            Color newColor = image.color;
            newColor.a = 1.0f;
            image.color = newColor;
        }
        // Map is Minimized --> Maximize Map
        else
        {
            if (image != null)
            {
                Color newColor = image.color;
                newColor.a = minimap_opacity;
                image.color = newColor;
            }

            // Center Map and Increaes Size
            minimapRectTransform.sizeDelta = maximizedSize;
            minimapRectTransform.anchoredPosition = centeredPosition;

            thirdPersonFollow.VerticalArmLength = maximizedZoom;
        }

        isMaximized = !isMaximized;
        //Debug.Log("Minimap Toggled");
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        // If the game isn't running, hide minimap
        if ((newState != GameState.PLAYING))
        {
            if (isMaximized)
            {
                ToggleMinimap();
            }
            minimap_canvas.gameObject.SetActive(false);
        }
        else
        {
            minimap_canvas.gameObject.SetActive(true);
        }

    }
}
