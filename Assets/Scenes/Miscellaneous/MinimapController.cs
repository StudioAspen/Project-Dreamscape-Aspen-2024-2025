using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

// TODO: Consolidate Logic into Two Paths

public class MinimapController : MonoBehaviour
{
    public RectTransform minimapRectTransform;
    private Vector2 normalSize;
    private Vector2 normalPosition;
    private Vector2 centeredPosition = new Vector2(0, 0);
    private bool isMaximized = false;
    private float originalZoom;

    [SerializeField] private Vector2 maximizedSize = new Vector2(300, 300);
    [SerializeField] private Camera m_Camera;
    [SerializeField] private int zoom = 80;
    [SerializeField] private float minimap_opacity = 0.75f;

    public void ToggleMinimap()
    {
        Mask mask = GetComponentInChildren<Mask>();
        if (mask != null) {
            mask.enabled = !mask.enabled;
            mask.GetComponent<Image>().enabled = !mask.GetComponent<Image>().enabled;
        }
        if (m_Camera.TryGetComponent(out UniversalAdditionalCameraData cameraData)) {
            cameraData.renderPostProcessing = !cameraData.renderPostProcessing;
        }
        if(transform.childCount > 0)
        {
            Transform border = transform.GetChild(0).Find("Border");
            if (border != null)
            {
                border.gameObject.SetActive(isMaximized);
            }
            else
            {
                Debug.Log("BORDER NOT FOUND?");
            }
        }

        if (isMaximized) {
            minimapRectTransform.sizeDelta = normalSize;
            minimapRectTransform.anchoredPosition = normalPosition;
        } else {
            minimapRectTransform.sizeDelta = maximizedSize;
            minimapRectTransform.anchoredPosition = centeredPosition;
        }

        CinemachineVirtualCamera virtualCamera = m_Camera.GetComponent<CinemachineVirtualCamera>();
        Cinemachine3rdPersonFollow thirdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if (isMaximized) {
            thirdPersonFollow.VerticalArmLength = originalZoom;
        } else {
            thirdPersonFollow.VerticalArmLength = zoom;
        }

        RawImage image = mask.GetComponentInChildren<RawImage>();
        if (!isMaximized) {
            if (image != null) {
                Color newColor = image.color;
                newColor.a = minimap_opacity;
                image.color = newColor;
            }
        } else {
            Color newColor = image.color;
            newColor.a = 1.0f;
            image.color = newColor;
        }


        isMaximized = !isMaximized;
    }
    // Start is called before the first frame update
    void Start()
    {
        normalSize = minimapRectTransform.sizeDelta;
        normalPosition = minimapRectTransform.anchoredPosition;

        RectTransform canvasRT = GetComponentInChildren<Canvas>().GetComponent<RectTransform>();
        centeredPosition = new Vector2(-(canvasRT.rect.width / 2), canvasRT.rect.height / 2);

        originalZoom = m_Camera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>().VerticalArmLength;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.M))
        {
            ToggleMinimap();
        }
    }
}
