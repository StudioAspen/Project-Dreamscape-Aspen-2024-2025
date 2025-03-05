using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SliderType {
    Volume,
    CameraSensitivity
}

public class SliderUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private PauseUIPanel pauseUI;
    private Slider slider;
    private TMP_Text sliderText;

    private string originalText = "";
    private Color originalColor;

    public Action OnButtonClicked = delegate { };

    private bool isSelected;
    private bool hovering;
    
    private void Awake()
    {
        pauseUI = GetComponentInParent<PauseUIPanel>();
        slider = GetComponent<Slider>();
        sliderText = GetComponentInChildren<TMP_Text>();
        originalText = sliderText.text;
        originalColor = sliderText.color;
    }

    private void OnEnable()
    {
        if(isSelected) EnableSelectedIndicator();
    }

    private void OnDisable()
    {
        isSelected = false;
        DisableSelectedIndicator();
    }

    private void Button_OnClick()
    {
        OnButtonClicked?.Invoke();
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

        EnableSelectedIndicator();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        DisableSelectedIndicator();
    }

    private void EnableSelectedIndicator() {
        hovering = true;
        sliderText.text = $">{originalText}<";
        sliderText.color = pauseUI.SliderHighlightColor;
    }

    private void DisableSelectedIndicator() {
        hovering = false;
        sliderText.text = originalText;
        sliderText.color = originalColor;
    }

    public void SetInteractable(bool isInteractable)
    {
        slider.interactable = isInteractable;
    }

    public bool IsInteractable()
    {
        return slider.interactable;
    }

    public void SetSliderOriginalText(string text) {
        originalText = text;
    }

    public void ForceUpdateText() {
        sliderText.text = hovering ? ">" + originalText + "<" : originalText;
    }
    
}
