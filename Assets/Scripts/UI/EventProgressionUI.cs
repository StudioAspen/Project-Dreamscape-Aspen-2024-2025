using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventProgressionUI : MonoBehaviour
{
    private EventManager eventManager;
    private ProgressionManager progressionManager;

    [System.Serializable]
    public class Quest
    {
        public TMP_Text Text;
        public Image TokenImage;
    }

    [Header("References")]
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Quest[] questsElements = new Quest[3];

    [Header("Assets")]
    [SerializeField] private Sprite empowerSprite;
    [SerializeField] private Sprite weakenSprite;

    // Safe to call in awake because UI loads last
    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        progressionManager = FindObjectOfType<ProgressionManager>();
    }

    private void Update()
    {
        UpdateEventElements();
        UpdateQuestElements();
    }

    private void UpdateEventElements()
    {
        if (eventManager == null) return;
        if(eventManager.CurrentEvent == null) return;
        eventManager.CurrentEvent.UpdateEventUIElements(feedbackText, nameText);
    }

    private void UpdateQuestElements()
    {
        for (int i = 0; i < progressionManager.CurrentQuests.Length; i++)
        {
            ProgressionQuestSO quest = progressionManager.CurrentQuests[i];

            if (quest == null)
            {
                questsElements[i].Text.text = "Quest N/A";
                questsElements[i].TokenImage.sprite = null;
            }
            else
            {
                questsElements[i].Text.text = quest.IsCompleted ? $"<s>{quest.ObjectiveText}</s>" : $"{quest.ObjectiveText}";
                questsElements[i].TokenImage.sprite = quest.CompletionReward == ProgressionQuestSO.Reward.EMPOWER_TOKEN ? empowerSprite : weakenSprite;
            }
        }
    }
}
