using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class EventSelectUIPanel : UIPanel
{
    private EventManager eventManager;

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

    private Coroutine startCardsAnimationCoroutine;

    private Type previousEvent = null;

    // UI scene loads last so Awake is safe here
    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();

        backgroundStartingColor = background.color;
        titleTextStartingColor = titleText.color;
    }

    public override void OnDeselected()
    {
        // Visually deselect all cards
        foreach (EventCardUI card in eventCards)
        {
            card.DisableSelectedIndicator();
        }
    }

    public void OnEnable()
    {
        AssignRandomEventsToCards();

        // Kill all previous tweens on the title and return it to its starting state
        titleText.DOKill();
        titleText.color = Color.clear;

        // Fade in title
        titleText.DOColor(titleTextStartingColor, backgroundFadeInDuration).SetUpdate(true);

        // Kill all previous tweens on the background and return it to its starting state
        background.DOKill();
        background.color = Color.clear;

        // Fade in background
        background.DOColor(backgroundStartingColor, backgroundFadeInDuration).SetUpdate(true).SetEase(Ease.InCubic);

        // Play the flipping cards animation
        PlayStartCardsAnimation();
    }

    public void OnDisable()
    {
        DisableCardButtons();

        foreach(EventCardUI card in eventCards)
        {
            card.InstantlyFlipCard(false);
        }
    }

    /// <summary>
    /// Plays the start animation by flipping event cards one after the other and enabling the cards at the end.
    /// </summary>
    private void PlayStartCardsAnimation()
    {
        if(startCardsAnimationCoroutine != null) StopCoroutine(startCardsAnimationCoroutine);
        startCardsAnimationCoroutine = StartCoroutine(StartCardsAnimationCoroutine());
    }

    private IEnumerator StartCardsAnimationCoroutine()
    {
        yield return null; // Wait a frame so that cards can initialize

        Vector3 bottomLeftPositionOffScreen = new Vector3(-Camera.main.pixelWidth, -Camera.main.pixelHeight, 0);

        // Kill all previous tweens on the event cards and return them to their starting states
        for (int i = 0; i < eventCards.Count; i++)
        {
            DOTween.Kill(eventCards[i]);
            eventCards[i].ResetCard();

            // Move the cards off screen
            eventCards[i].transform.position = bottomLeftPositionOffScreen;
        }

        for (int i = 0; i < eventCards.Count; i++)
        {
            Sequence sequence = DOTween.Sequence().SetUpdate(true).SetId(eventCards[i]);
            sequence.Append(eventCards[i].MoveToStartingPosition(cardDealDuration, Ease.OutCubic).SetUpdate(true));
            sequence.Append(eventCards[i].FlipCard(enterCardsFlipDuration, true).SetUpdate(true));
            if (i == eventCards.Count - 1)
            {
                sequence.OnComplete(EnableCardButtons);
            }
            sequence.Play();

            yield return new WaitForSecondsRealtime(flipDurationBetweenCards);
        }

        startCardsAnimationCoroutine = null;
    }

    /// <summary>
    /// Assigns random events to each event card.
    /// </summary>
    private void AssignRandomEventsToCards()
    {
        List<Type> potentialEvents = new List<Type>(eventManager.EventsDictionary.Keys);

        // Prevent players from selecting the same event two waves in a row.
        if (previousEvent != null) 
        {
          Debug.Log($"Previous Event: {previousEvent.Name}\n Now Removing {previousEvent.Name}...");
          Type type = potentialEvents.Find(x => x == previousEvent);
          potentialEvents.Remove(type);
        }

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

        previousEvent = clickedCard.CurrentEventType;

        AkSoundEngine.PostEvent("CardSelect", clickedCard.gameObject);
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
            card.FlipCard(nonSelectedCardsExitFlipDuration, false).OnComplete(() =>
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

                    clickedCard.FlipCard(selectedCardExitFlipDuration, false).OnComplete(() =>
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
