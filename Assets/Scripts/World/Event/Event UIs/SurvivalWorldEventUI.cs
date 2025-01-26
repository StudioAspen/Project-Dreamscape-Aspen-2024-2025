using TMPro;
using UnityEngine;

public class SurvivalWorldEventUI : WorldEventUI
{
    private SurvivalWorldEventSO survivalWorldEvent;

    [SerializeField] private TMP_Text timerText;

    private protected override void OnAwake()
    {
        survivalWorldEvent = GetAndValidateCurrentEvent<SurvivalWorldEventSO>();
    }

    private protected override void OnOnDestroy()
    {
        
    }

    private void Update()
    {
        timerText.text = $"{Mathf.Round(survivalWorldEvent.RemainingTime)}s";
    }
}
