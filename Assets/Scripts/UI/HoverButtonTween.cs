using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButtonTween : HoverUI
{
    [Header("Config")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float tweenDuration = 0.1f;
    [SerializeField] private Ease tweenEase = Ease.OutCubic;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private protected override void OnOnEnable()
    {
        
    }

    private protected override void OnOnSelected(BaseEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale * hoverScale, tweenDuration).SetEase(tweenEase).SetUpdate(true);
    }

    private protected override void OnOnDeselected(BaseEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, tweenDuration).SetEase(tweenEase).SetUpdate(true);
    }
}

