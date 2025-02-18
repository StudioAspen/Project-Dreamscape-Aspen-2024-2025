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
            displayText.color = Color.red;
            displayText.text = $"Failed";
            return;
        }

        displayText.text = $"({Mathf.Max(defendWorldEvent.DefendEventEntity.CurrentHealth, 0)}/{defendWorldEvent.DefendEventEntity.MaxHealth.GetIntValue()}): {Mathf.Round(defendWorldEvent.RemainingTime)}s";
    }
}