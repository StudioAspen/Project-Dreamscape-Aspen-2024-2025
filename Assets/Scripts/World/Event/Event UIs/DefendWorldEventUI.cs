using TMPro;
using UnityEngine;

public class DefendWorldEventUI : WorldEventUI
{
    private DefendWorldEventSO defendWorldEvent;

    [SerializeField] private TMP_Text displayText;

    private protected override void OnAwake()
    {
        defendWorldEvent = GetAndValidateCurrentEvent<DefendWorldEventSO>();
    }

    private protected override void OnOnDestroy()
    {
        
    }

    private void Update()
    {
        if(defendWorldEvent.DefendEventEntity == null)
        {
            displayText.text = $"Failed";
            return;
        }

        displayText.text = $"({defendWorldEvent.DefendEventEntity.CurrentHealth}/{defendWorldEvent.DefendEventEntity.MaxHealth}): {Mathf.Round(defendWorldEvent.RemainingTime)}s";
    }
}