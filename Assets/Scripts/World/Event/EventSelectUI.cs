using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class EventSelectUI : MonoBehaviour
{
    private GameManager gameManager;
    private EventManager eventManager;
    private Image panel;

    [Header("References")]
    [SerializeField] private List<EventCardUI> eventCards;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        eventManager = FindObjectOfType<EventManager>();
        panel = GetComponent<Image>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.EVENT_SELECTION)
        {
            Disable();
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
        DisableCards();

        gameObject.SetActive(false);
    }

    private void AssignRandomEventsToCards()
    {
        List<Type> potentialEvents = new List<Type>(eventManager.EventsDictionary.Keys);

        foreach (EventCardUI card in eventCards)
        {
            int randomIndex = UnityEngine.Random.Range(0, potentialEvents.Count);

            Type randomEvent = potentialEvents[randomIndex];

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
