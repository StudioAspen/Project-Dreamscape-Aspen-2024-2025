using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatPanel : MonoBehaviour 
{
    // public Stat stat;
    [SerializeField] private Stat stat;
    [SerializeField] private TMP_Text StatPanelText;
    
    // Update is called once per frame
    void Update()
    {
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        // for debugging
        // if (stat == null || StatPanelText == null)
        // {
        //     Debug.Log("Stat: " + stat + "StatPanelText: " + StatPanelText);
        //     return;
        // }

        // float baseValue = stat.BaseValue;
        // float flatIncrease = stat.GetTotalFlatIncreass();
        // float totalMultipliers = stat.GetTotalMultiplier();


        string displayText = $"Base Value: {stat.baseValue}\n" +
                             $"MaxHealth: {stat.MaxHealth}" +
                             $"StatusSpeedModifier: {stat.StatusSpeedModifier}" +
                             $"DamageModifier: {stat.DamageModifier}" +
                             $"DebufSpeedMultiplier: {stat.DebufSpeedMultiplier}" +
                             $"LocalTimeScale: {stat.LocalTimeScale}" +
                             $"SizeScale: {stat.SizeScale}" + 

                             $"Total Multipliers: {totalMultipliers}\n" +
                             $"Flat Increase: {flatIncrease}";

        StatPanelText.text = displayText;
    }
}
