using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private PauseUIPanel pauseUI;
    private Button button;
    private TMP_Text buttonText;

    private string originalText = "";
    private Color originalColor;

    public Action OnButtonClicked = delegate { };

    private bool isSelected;

    private void Start()
    {
        pauseUI = GetComponentInParent<PauseUIPanel>();
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>();

        originalText = buttonText.text;
        originalColor = buttonText.color;

        button.onClick.AddListener(Button_OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(Button_OnClick);
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

    private void EnableSelectedIndicator()
    {
        buttonText.text = $"> {originalText} <";
        buttonText.color = pauseUI.ButtonHighlightColor;
    }

    private void DisableSelectedIndicator()
    {
        buttonText.text = originalText;
        buttonText.color = originalColor;
    }

    public void SetInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;
    }

    public bool IsInteractable()
    {
        return button.interactable;
    }
}
