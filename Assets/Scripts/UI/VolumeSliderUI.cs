using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSliderUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
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

    private void Update() {
        sliderText.text = (hovering ? ">" : "") + "Volume: " + Mathf.Floor(slider.value * 100f) + "%" + (hovering ? "<" : "");
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
        sliderText.color = pauseUI.SliderHighlightColor;
    }

    private void DisableSelectedIndicator() {
        hovering = false;
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
}
