using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening;
using TMPro;

public class EventSelectUI : MonoBehaviour
{
    private GameManager gameManager;
    private EventManager eventManager;
    private Image panel;

    [Header("References")]
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private List<EventCardUI> eventCards;

    [Header("On Enter Config")]
    [SerializeField] private float backgroundFadeInDuration = 0.5f;
    [SerializeField] private float flipDurationBetweenCards = 0.25f;
    [SerializeField] private float enterCardsFlipDuration = 0.25f;
    [SerializeField] private float cardDealDuration = 0.25f;

    [Header("On Exit Config")]
    [SerializeField] private float selectedCardCenteringDuration = 0.5f;
    [SerializeField] private float selectedCardViewingDuration = 1f;
    [SerializeField] private float cardSlideDownDuration = 0.5f;
    [SerializeField] private float nonSelectedCardsExitFlipDuration = 0.25f;
    [SerializeField] private float selectedCardExitFlipDuration = 0.25f;
    private float backgroundFadeOutDuration => selectedCardExitFlipDuration + cardSlideDownDuration;

    private Color backgroundStartingColor;
    private Color titleTextStartingColor;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        eventManager = FindObjectOfType<EventManager>();
        panel = GetComponent<Image>();

        backgroundStartingColor = background.color;
        titleTextStartingColor = titleText.color;

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
        
        titleText.color = titleTextStartingColor;

        // Kill all previous tweens on the background and return it to its starting state
        background.DOKill();
        background.color = Color.clear;

        // Fade in background
        background.DOColor(backgroundStartingColor, backgroundFadeInDuration).SetUpdate(true).SetEase(Ease.InCubic);
        
        // Play the flipping cards animation
        PlayStartAnimation();
    }

    public void Disable()
    {
        DisableCardButtons();

        foreach(EventCardUI card in eventCards)
        {
            card.InstantlyFlipCard(false);
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Plays the start animation by flipping event cards one after the other and enabling the cards at the end.
    /// </summary>
    private void PlayStartAnimation()
    {
        // Kill all previous tweens on the event cards and return them to their starting states
        for (int i = 0; i < eventCards.Count; i++)
        {
            DOTween.Kill(eventCards[i]);
            eventCards[i].ResetCard();
        }

        Vector3 bottomLeftPositionOffScreen = new Vector3(-Camera.main.pixelWidth, -Camera.main.pixelHeight, 0);

        for (int i = 0; i < eventCards.Count; i++)
        {
            // Move the cards off screen
            eventCards[i].transform.position = bottomLeftPositionOffScreen;

            int localIndex = i;

            if (i == eventCards.Count - 1)
            {
                PlayCardAnimationByIndex(localIndex, EnableCardButtons);
                break;
            }

            PlayCardAnimationByIndex(localIndex);
        }

        void PlayCardAnimationByIndex(int index, Action onCompleteCallback = null)
        {
            DOVirtual.DelayedCall(index * flipDurationBetweenCards, () =>
            {
                eventCards[index].MoveToStartingPosition(cardDealDuration, Ease.OutCubic, () => {
                    eventCards[index].FlipCard(enterCardsFlipDuration, true, onCompleteCallback);
                });
            }).SetId(eventCards[index]);
        }
    }

    /// <summary>
    /// Assigns random events to each event card.
    /// </summary>
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

    private void Card_OnCardClicked(EventCardUI clickedCard)
    {
        DisableCardButtons();

        PlayExitAnimation(clickedCard);
    }

    /// <summary>
    /// Plays the exit animation when a card is clicked.
    /// </summary>
    /// <param name="clickedCard">The clicked card.</param>
    private void PlayExitAnimation(EventCardUI clickedCard)
    {
        foreach (EventCardUI card in eventCards)
        {
            if (card == clickedCard) continue;

            // Flip and slide down the non-selected cards
            card.FlipCard(nonSelectedCardsExitFlipDuration, false, () =>
            {
                card.transform.DOMoveY(-Camera.main.pixelHeight, cardSlideDownDuration).SetUpdate(true).SetEase(Ease.InOutBack);
            });
        }

        // Fade the title away
        titleText.DOColor(Color.clear, cardSlideDownDuration).SetUpdate(true);

        // Center and scale the selected card and then slide it down after a delay
        DOVirtual.DelayedCall(cardSlideDownDuration, () =>
        {
            clickedCard.transform.DOScale(1.2f, selectedCardCenteringDuration).SetEase(Ease.OutBack).SetUpdate(true); // Scale it

            Vector3 screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);
            clickedCard.transform.DOMove(screenCenter, selectedCardCenteringDuration).SetUpdate(true).SetEase(Ease.OutQuint).OnComplete(() => // Center it
            {
                DOVirtual.DelayedCall(selectedCardViewingDuration, () => // Wait for a bit to let the player read the card
                {
                    background.DOColor(Color.clear, backgroundFadeOutDuration).SetUpdate(true).SetEase(Ease.InCubic); // Fade out the background

                    clickedCard.FlipCard(selectedCardExitFlipDuration, false, () => // Flip it
                    {
                        // Slide it down
                        clickedCard.transform.DOMoveY(-Camera.main.pixelHeight, cardSlideDownDuration).SetUpdate(true).SetEase(Ease.InOutBack).OnComplete(() =>
                        {
                            eventManager.ChangeEvent(clickedCard.CurrentEventType); // Change the event
                        });
                    });
                });
            });
        });
    }

    private void EnableCardButtons()
    {
        foreach (EventCardUI card in eventCards)
        {
            card.EnableButton();

            card.OnCardClicked += Card_OnCardClicked;
        }
    }

    private void DisableCardButtons()
    {
        foreach (EventCardUI card in eventCards)
        {
            card.DisableButton();

            card.OnCardClicked -= Card_OnCardClicked;
        }
    }
}
