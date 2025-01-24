using TMPro;
using UnityEngine;

public class EscortWorldEventUI : WorldEventUI
{
    private EscortWorldEventSO escortWorldEvent;

    [SerializeField] private TMP_Text displayText;

    private protected override void OnAwake()
    {
        escortWorldEvent = GetAndValidateCurrentEvent<EscortWorldEventSO>();
    }

    private protected override void OnOnDestroy()
    {

    }

    private void Update()
    {
        if (escortWorldEvent.EscortEventEntity == null)
        {
            displayText.text = $"Failed";
            return;
        }

        displayText.text = $"({escortWorldEvent.EscortEventEntity.CurrentHealth}/{escortWorldEvent.EscortEventEntity.MaxHealth}): {Mathf.Round(escortWorldEvent.RemainingTime)}s";
    }
}
