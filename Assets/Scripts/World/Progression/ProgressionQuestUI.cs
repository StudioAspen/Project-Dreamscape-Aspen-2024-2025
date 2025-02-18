using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressionQuestUI : MonoBehaviour
{
    private ProgressionManager progressionManager;

    [SerializeField] private TMP_Text[] questTexts = new TMP_Text[3];

    private void Start()
    {
        progressionManager = FindObjectOfType<ProgressionManager>();
    }

    private void Update()
    {
        UpdateQuestTexts();
    }

    private void UpdateQuestTexts()
    {
        for (int i = 0; i < progressionManager.CurrentQuests.Length; i++)
        {
            ProgressionQuestSO quest = progressionManager.CurrentQuests[i];

            if (quest == null)
            {
                questTexts[i].text = "Quest N/A";
                questTexts[i].color = Color.red;
            }
            else
            {
                questTexts[i].text = $"{quest.ObjectiveText} = {(quest.CompletionReward == ProgressionQuestSO.Reward.EMPOWER_TOKEN ? "Empower" : "Weaken")} token";
                questTexts[i].color = quest.IsCompleted ? Color.green : Color.red;
            }
        }
    }
}
