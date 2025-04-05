using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class StatPanel : MonoBehaviour 
{
    public Player player;
    public List<Stat> statList;
    public List<TMP_Text> DisplayText;

    // Update is called once per frame
    private void Awake()
    {
        player = FindObjectOfType<Player>().GetComponent<Player>();
        statList.Add(player.MaxHealth);
        statList.Add(player.StatusSpeedModifier);
        statList.Add(player.DamageModifier);
        statList.Add(player.LocalTimeScale);
        statList.Add(player.SizeScale);
        statList.Add(player.Defense);
        statList.Add(player.DebuffApplyDurationMultiplier);
        statList.Add(player.BuffApplyDurationMultiplier);

    }

    private void Start()
    {
        /*displayStats();*/
    }

    private void OnEnable()
    {
        /*For Some reason is called twice? and doubles up displayed values*/
        displayStats();
    }

    private void displayStats()
    {
        for(int i = 0; i < statList.Count; i++)
        {
            DisplayText[i].text = statList[i].DisplayName;
            DisplayText[i].text += ": ";
            DisplayText[i].text += statList[i].GetIntValue().ToString();
            DisplayText[i].text += ", ";
            DisplayText[i].text += statList[i].BaseValue.ToString();
            DisplayText[i].text += ", ";
            DisplayText[i].text += statList[i].GetTotalMultiplier().ToString();
            DisplayText[i].text += ", ";
            DisplayText[i].text += statList[i].GetTotalFlatIncreass().ToString();
            
        }
    }
}
