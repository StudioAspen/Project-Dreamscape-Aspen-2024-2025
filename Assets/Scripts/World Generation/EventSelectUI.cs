using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        Disable();
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
        gameObject.SetActive(false);

        DisableCards();
    }

    private void AssignRandomEventsToCards()
    {
        foreach (EventCardUI card in eventCards)
        {
            WorldEvent randomEvent = (WorldEvent)Random.Range(0, System.Enum.GetValues(typeof(WorldEvent)).Length);

            card.AssignCardBiome(randomEvent);
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
