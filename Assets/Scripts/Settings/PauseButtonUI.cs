using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private TMP_Text buttonText;

    private string originalText = "";

    public Action OnButtonClicked = delegate { };

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>();

        originalText = buttonText.text;

        button.onClick.AddListener(Button_OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(Button_OnClick);
    }

    private void Button_OnClick()
    {
        OnButtonClicked?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.text = $"<> {originalText} <>";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.text = originalText;
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
