using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PauseUI pauseUI;
    private Button button;
    private TMP_Text buttonText;

    private string originalText = "";
    private Color originalColor;

    public Action OnButtonClicked = delegate { };

    private void Awake()
    {
        pauseUI = GetComponentInParent<PauseUI>();
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
        buttonText.text = originalText;
        buttonText.color = originalColor;
    }

    private void Button_OnClick()
    {
        OnButtonClicked?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.text = $"> {originalText} <";
        buttonText.color = pauseUI.ButtonHighlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
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
