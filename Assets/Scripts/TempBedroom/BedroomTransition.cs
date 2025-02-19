using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BedroomTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Light directionalLight;
    public Color baseColor; // The target color when hovering
    private static Color lastColor; // Stores the last hovered color
    private bool isHovering = false;
    private Tween pulseTween; // Tween for pulsing effect

    [Header("Pulse Settings")]
    public float pulseMinIntensity = 0.9f; // Slightly dimmer
    public float pulseMaxIntensity = 1.1f; // Slightly brighter
    public float pulseSpeed = 2f; // Slower pulse for natural feel

    private void Start()
    {
        // Set default color if no previous color exists
        if (lastColor == default)
            lastColor = directionalLight.color;
        else
            directionalLight.color = lastColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Stop any existing pulse effect
        pulseTween?.Kill();

        // Smoothly transition to the target color
        directionalLight.DOColor(baseColor, 0.7f).SetEase(Ease.InOutSine);

        // Start the slow pulsing effect
        pulseTween = DOTween.To(() => directionalLight.intensity,
                                x => directionalLight.intensity = x,
                                pulseMaxIntensity, pulseSpeed) // Slower, natural feel
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine);

        // Store the last hovered color
        lastColor = baseColor;
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        // Stop pulsing but keep the last color
        pulseTween?.Kill();
        directionalLight.DOIntensity(1f, 1f).SetEase(Ease.InOutSine);
    }
}
