using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private RectTransform healthDifferenceFill;
    private RectTransform rectTransform;
    private TMP_Text healthText;

    [Header("Settings")]
    [SerializeField] private float healthDifferenceFillDelay = 0.5f;
    [SerializeField] private float healthDifferenceFillDuration = 0.5f;

    private float value;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        healthText = GetComponentInChildren<TMP_Text>();
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
        healthDifferenceFill.DOSizeDelta(healthFill.sizeDelta, healthDifferenceFillDuration).SetUpdate(true).SetDelay(healthDifferenceFillDelay).SetEase(Ease.OutQuint);
    }
}
