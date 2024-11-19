using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EventSelectUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Self] private Image panel;
    [SerializeField] private List<EventCardUI> eventCards;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        Disable();
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        Disable();
        if (newState != GameState.EVENT_SELECTION)
        {
            return;
        }

        Enable();
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        AssignRandomEventsToCards();

        EnableCards();
    }

    public void Disable()
    {
        gameObject.SetActive(false);

        DisableCards();
    }

    private void AssignRandomEventsToCards()
    {
        List<WorldEvent> potentialEvents = System.Enum.GetValues(typeof(WorldEvent)).Cast<WorldEvent>().ToList();

        foreach (EventCardUI card in eventCards)
        {
            int randomIndex = Random.Range(1, potentialEvents.Count);

            WorldEvent randomEvent = potentialEvents[randomIndex];

            card.AssignCardEvent(randomEvent);

            potentialEvents.RemoveAt(randomIndex);
        }
    }

    private void EnableCards()
    {
        foreach (EventCardUI card in eventCards)
        {
            card.EnableButton();
        }
    }

    private void DisableCards()
    {
        foreach (EventCardUI card in eventCards)
        {
            card.DisableButton();
        }
    }
}
