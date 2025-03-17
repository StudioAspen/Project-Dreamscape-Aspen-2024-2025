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

    public string originalText { get; private set; }
    private Color originalColor;
    

    public Action OnButtonClicked = delegate { };

    private bool isSelected;

    private void Awake()
    {
        pauseUI = GetComponentInParent<PauseUIPanel>();
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>();

        originalText = buttonText.text;
        originalColor = buttonText.color;
    }

    private void Start()
    {
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

    public void OnPointerEnter(PointerEventData eventData) {
        OnSelect(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselect(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!button.interactable) return;
        isSelected = true;

        EnableSelectedIndicator();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!button.interactable) return;
        isSelected = false;

        DisableSelectedIndicator();
    }

    private void EnableSelectedIndicator() {
        buttonText.text = $"> {buttonText.text} <";
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

        if (isInteractable) {
            UnGreyOut();
        } else {
            GreyOut();
        }
        
    }

    public bool IsInteractable()
    {
        return button.interactable;
    }

    public void SetOriginalText(string origTxt) {
        originalText = origTxt;
    }

    private void GreyOut() {
        buttonText.color = new Color(47 / 255f, 47 / 255f, 47 / 255f);
    }

    private void UnGreyOut() {
        buttonText.color = originalColor;
    }
    
}
