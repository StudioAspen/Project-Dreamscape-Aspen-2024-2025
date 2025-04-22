using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private EventManager eventManager;

    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text selectText;
    [SerializeField] private GameObject backgroundGlow;
    [SerializeField] private GameObject frontFaceObject;
    [SerializeField] private GameObject backFaceObject;

    private bool isFrontFacing;
    private bool isSelected;

    public Type CurrentEventType { get; private set; }
    /// <summary>
    /// Action that is invoked when the card is clicked.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item><description><c>EventCardUI card</c>: The card that was clicked.</description></item>
    /// </list>
    /// </remarks>
    public Action<EventCardUI> OnCardClicked = delegate { };

    private Vector3 startingPosition;

    // Awake is safe here because UI scene loads last
    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();

        startingPosition = transform.localPosition;

        ResetCard();
    }

    private void Start()
    {
        button.onClick.AddListener(Button_OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(Button_OnClick);
    }

    private void OnDisable()
    {
        isSelected = false;
    }

    public void EnableButton()
    {
        button.interactable = true;

        // Check if the pointer is over the card when it is enabled
        if (isSelected) EnableSelectedIndicator();
    }

    public void DisableButton()
    {
        button.interactable = false;
    }

    /// <summary>
    /// Resets the position of the card to default state.
    /// </summary>
    public void ResetCard()
    {
        DisableSelectedIndicator();
        InstantlyFlipCard(false);

        transform.localPosition = startingPosition;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Assigns the given event type to the card and updates the card visuals.
    /// </summary>
    /// <param name="eventType">The type of the event to assign.</param>
    public void AssignCardEvent(Type eventType)
    {
        CurrentEventType = eventType;

        if (eventManager == null) eventManager = FindObjectOfType<EventManager>();

        WorldEventSO worldEvent = eventManager.GetEvent(CurrentEventType);

        nameText.text = $"{worldEvent.EventName}";
        descriptionText.text = $"{worldEvent.Description}";
    }

    private void Button_OnClick()
    {
        OnCardClicked?.Invoke(this);
    }

    #region Hovering/Selecting
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselect(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;

        if (!button.interactable) return;

        EnableSelectedIndicator();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        if (!button.interactable) return;

        DisableSelectedIndicator();
    }

    /// <summary>
    /// Enables the selected indicator for the event card.
    /// </summary>
    public void EnableSelectedIndicator()
    {
        selectText.text = "<b>> SELECT <<b>";

        backgroundGlow.SetActive(true);
    }

    /// <summary>
    /// Disables the selected indicator for the event card.
    /// </summary>
    public void DisableSelectedIndicator()
    {
        selectText.text = "SELECT";

        backgroundGlow.SetActive(false);
    }
    #endregion

    #region Flipping
    /// <summary>
    /// Flips the card to the specified side and executes a callback when the animation is complete.
    /// </summary>
    /// <param name="flipDuration"></param>
    /// <param name="isFront">True to flip the card to the front side, false to flip it to the back side.</param>
    public Sequence FlipCard(float flipDuration, bool isFront)
    {
        if (isFrontFacing == isFront)
        {
            // If the card is already facing the specified side, don't do anything.
            return null;
        }

        // Kill existing tweens
        DOTween.Kill(gameObject);

        // Create a sequence
        Sequence flipSequence = DOTween.Sequence().SetUpdate(true).SetId(gameObject);

        // Add the first half of the flip (0 to 90 degrees)
        flipSequence.Append(transform.DOLocalRotate(new Vector3(0f, 90f, 0f), flipDuration / 2f).SetUpdate(true)
            .OnComplete(() => {
                // Toggle faces halfway
                frontFaceObject.SetActive(isFront);
                backFaceObject.SetActive(!isFront);
            }));

        // Add the second half of the flip (90 to 180 degrees or back to 0)
        flipSequence.Append(transform.DOLocalRotate(isFront ? Vector3.zero : new Vector3(0f, 180f, 0f), flipDuration / 2f).SetUpdate(true))
            .OnComplete(() => {
                // Ensure the card is facing the correct side
                InstantlyFlipCard(isFront);
            });

        return flipSequence.Play();
    }

    /// <summary>
    /// Instantly flips the card to the specified side.
    /// </summary>
    /// <param name="isFront">True to flip the card to the front side, false to flip it to the back side.</param>
    public void InstantlyFlipCard(bool isFront)
    {
        frontFaceObject.SetActive(isFront);
        backFaceObject.SetActive(!isFront);

        transform.localEulerAngles = isFront ? Vector3.zero : new Vector3(0f, 180f, 0f);

        isFrontFacing = isFront;
    }
    #endregion

    #region Movement
    /// <summary>
    /// Moves the card from the start position to the end position over a specified duration.
    /// </summary>
    /// <param name="endPosition">The starting position of the card.</param>
    /// <param name="duration">The duration of the movement.</param>
    public Tween MoveToStartingPosition(float duration, Ease ease)
    {
        // Kill existing tween
        DOTween.Kill(transform);

        // Move the card to the end position (starting original position)
        return transform.DOLocalMove(startingPosition, duration).SetUpdate(true).SetEase(ease);
    }
    #endregion
}
