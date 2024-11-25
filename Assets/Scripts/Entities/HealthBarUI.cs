using DG.Tweening;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private RectTransform rectTransform;
    [SerializeField, Child] private TMP_Text healthText;
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private RectTransform healthDifferenceFill;

    [Header("Settings")]
    [SerializeField] private float healthDifferenceFillDelay = 0.5f;
    [SerializeField] private float healthDifferenceFillDuration = 0.5f;

    private float value;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Start()
    {
        value = 1;
        healthText.text = "0/0";

        healthDifferenceFill.sizeDelta = healthFill.sizeDelta;
    }

    public void SetHealthBar(int currentValue, int maxValue)
    {
        value = (float)Mathf.Clamp(currentValue, 0, maxValue) / maxValue;

        healthFill.sizeDelta = new Vector2(value * rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);

        healthText.text = $"{Mathf.Clamp(currentValue, 0, maxValue)}/{maxValue}";

        DOTween.Kill(healthDifferenceFill);
        healthDifferenceFill.DOSizeDelta(healthFill.sizeDelta, healthDifferenceFillDuration).SetUpdate(true).SetDelay(healthDifferenceFillDelay).SetEase(Ease.OutQuint).OnUpdate(() => { Debug.Log($"{healthFill.sizeDelta}, {healthDifferenceFill.sizeDelta}"); });
    }
}
