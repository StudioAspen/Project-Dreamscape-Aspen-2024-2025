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
            displayText.color = Color.red;
            displayText.text = $"Failed";
            return;
        }

        displayText.text = $"({Mathf.Max(escortWorldEvent.EscortEventEntity.CurrentHealth, 0)}/{escortWorldEvent.EscortEventEntity.MaxHealth}): {Mathf.Round(escortWorldEvent.RemainingTime)}s";
    }
}
