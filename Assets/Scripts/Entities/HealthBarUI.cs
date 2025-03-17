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
    private RectTransform rectTransform;
    private TMP_Text healthText;

    private float value;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        healthText = GetComponentInChildren<TMP_Text>();

        value = 1;
        healthText.text = "0/0";
    }

    /// <summary>
    /// Sets the health bar based on the current and maximum values.
    /// If the maximum value is 0, the health bar is set to full and displays "Invincible".
    /// </summary>
    /// <param name="currentValue">The current value of the health bar.</param>
    /// <param name="maxValue">The maximum value of the health bar.</param>
    public void SetHealthBar(int currentValue, int maxValue)
    {
        if (maxValue == 0)
        {
            value = 1; // Make bar full
            healthFill.sizeDelta = new Vector2(value * rectTransform.sizeDelta.x, rectTransform.sizeDelta.y); // Update fill size
            healthText.text = "Invincible"; // Update text
            return;
        }

        value = (float)Mathf.Clamp(currentValue, 0, maxValue) / maxValue;

        healthFill.sizeDelta = new Vector2(value * rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);

        healthText.text = $"{Mathf.Clamp(currentValue, 0, maxValue)}/{maxValue}";
    }
}
