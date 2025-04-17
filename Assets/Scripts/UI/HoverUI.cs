using UnityEngine;
using UnityEngine.EventSystems;

public abstract class HoverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private protected bool isSelected;

    private void OnEnable()
    {
        if (isSelected) OnSelect(null);
        OnOnEnable();
    }

    private protected abstract void OnOnEnable();

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

        OnOnSelected(eventData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        OnOnDeselected(eventData);
    }

    private protected abstract void OnOnSelected(BaseEventData eventData);

    private protected abstract void OnOnDeselected(BaseEventData eventData);
}
