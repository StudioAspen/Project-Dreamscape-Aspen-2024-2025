using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButtonTween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Config")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float tweenDuration = 0.1f;
    [SerializeField] private Ease tweenEase = Ease.OutCubic;

    private Vector3 originalScale;
    private bool isSelected;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        if(isSelected) OnSelect(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselect(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;

        transform.DOKill();
        transform.DOScale(originalScale * hoverScale, tweenDuration).SetEase(tweenEase).SetUpdate(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        transform.DOKill();
        transform.DOScale(originalScale, tweenDuration).SetEase(tweenEase).SetUpdate(true);
    }
}
