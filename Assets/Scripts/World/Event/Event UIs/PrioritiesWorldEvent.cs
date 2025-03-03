using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrioritiesWorldEventUI : WorldEventUI
{
    private PrioritiesWorldEventSO prioritiesWorldEvent;



    [SerializeField] private TMP_Text displayText;
    // Start is called before the first frame update
    private protected override void OnStart()
    {
        prioritiesWorldEvent = GetAndValidateCurrentEvent<PrioritiesWorldEventSO>();
    }

    private protected override void OnOnDestroy()
    {

    }

    private void Update()
    {
        if (prioritiesWorldEvent.OnPrioritiesEnemyDeath == null)
        {
            displayText.color = Color.red;
            displayText.text = $"Bye Bye ";
            return;
        }
    }
}
