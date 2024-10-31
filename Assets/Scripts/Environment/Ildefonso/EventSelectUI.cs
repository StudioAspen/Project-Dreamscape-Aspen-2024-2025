using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//SCRIPT IS A DUPLICATE OF IslandSelectUI.cs,
//I ILDEFONSO MARRERO BRANCHING OFF OF THAT SCRIPT TO IMPLEMENT THE EVENT SELECT UI
//I DO NOT TAKE CREDIT FOR MOST OF THIS CODE

public class EventSelectUI : MonoBehaviour
{
    [Header("References")]
    private EventManager eventManager;
    private WorldManager worldManager;
    private MasterLevelManager masterLevelManager;

    [SerializeField]
    public EventType[] availableEvents; // Array of available events
    public Button[] isSelectPlaceHolder; // UI Buttons to display the events
    public float animationDuration = 1f; // Duration of the animation
    public float spacing = 10f;  // Space between buttons
    public float bounceDuration = 5f; // Duration of the bounce

    private void Awake()
    {
        eventManager = FindObjectOfType<EventManager>();
        worldManager = FindObjectOfType<WorldManager>();
        masterLevelManager = FindObjectOfType<MasterLevelManager>();
    }

    void Start()
    {
        // Ensure all buttons are not active and not shown to the player
        foreach (Button button in isSelectPlaceHolder)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void Update()
    {

    }

    public void PrepareEventSelection()
    {
        Debug.Log("Preparing Event Select");
        Time.timeScale = 0f; // Freeze the game

        Cursor.lockState = CursorLockMode.None; // Unlock the mouse
        Cursor.visible = true; // Show the cursor
        ///deal 4 random islands 
        EventType[] selectedEvents = GetEvents(4);

        //Set up buttons with selected islands 
        for (int i = 0; i < isSelectPlaceHolder.Length; i++)
        {
            ///Set the button text and image 
            Button button = isSelectPlaceHolder[i];
            EventType selected_event = selectedEvents[i];
            button.GetComponentInChildren<TMP_Text>().text = selected_event.ToString();
            //button.image.sprite = event.eventImage; // Set button image if you have one
            button.interactable = false;

            // Set initial position to a random off-screen point
            Vector3 randomOffScreenPosition = new Vector3(Random.Range(-Screen.width * 2, 5000), Random.Range(-2000, 2000), 0);
            button.transform.localPosition = randomOffScreenPosition;

            // Calculate target position for row alignment
            float targetXPosition = i * spacing * 50; // Align in a row
            Vector3 targetPosition = new Vector3(targetXPosition - 750, 0, 0); // Adjust X offset as needed

            // Create a sequence for smooth movement with a bounce effect
            Sequence buttonSequence = DOTween.Sequence();
            buttonSequence.Append(button.transform.DOLocalMove(targetPosition, animationDuration).SetEase(Ease.OutExpo)).SetUpdate(true);
            buttonSequence.Append(button.transform.DOLocalMoveY(100, bounceDuration).SetEase(Ease.OutBounce)).SetUpdate(true); // Bounce up
            buttonSequence.Append(button.transform.DOLocalMoveY(0, bounceDuration).SetEase(Ease.OutBounce)).SetUpdate(true); // Bounce down
            buttonSequence.SetDelay(i * 0.25f); // Stagger the animations by 0.2 seconds
            buttonSequence.OnComplete(() => { button.interactable = true; });


            // Add listener for selection
            button.gameObject.SetActive(true); // Enable the button here
            button.onClick.AddListener(() => OnButtonSelected(button, selected_event));
        }
    }

    EventType[] GetEvents(int count)
    {
        EventType[] selectedEvents = new EventType[count];
        List<EventType> availableList = new List<EventType>(availableEvents);

        // Shuffle the list of available events
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableList.Count);
            selectedEvents[i] = availableList[randomIndex];
            availableList.RemoveAt(randomIndex);
        }

        Debug.Log("Selected Events: " + string.Join(", ", selectedEvents));
        return selectedEvents;
    }

    public void OnButtonSelected(Button selectedButton, EventType selectedEvent)
    {
        Time.timeScale = 1f;

        Debug.Log(selectedEvent.ToString() + " selected!");

        // Move the selected button to the top middle of the screen
        Vector3 targetPosition = new Vector3(0, Screen.height / 2, 0); // Adjust Y position as needed

        // Animate the selected button
        selectedButton.transform.DOLocalMove(targetPosition, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

        // Perform the action based on the selected event
        eventManager.SetCurrentEvent(selectedEvent);

        // Hide other buttons
        foreach (Button button in isSelectPlaceHolder)
        {
            button.interactable = false;
            if (button != selectedButton)
            {
                button.gameObject.SetActive(false);
            }
        }

        //if VISIT_ALL event is selected, reset all islands to unvisited
        if (selectedEvent == EventType.VISIT_ALL)
        {
            foreach (IslandManager island in masterLevelManager.SpawnedIslands)
            {
                island.IsVisited = false;
            }
        }

        // set IsEventSelecting to false since we are done selecting
        worldManager.IsEventSelecting = false;
    }

    public void RemoveAllCards()
    {
        for (int i = 0; i < isSelectPlaceHolder.Length; i++)
        {
            ///Disable and remove functionality of all buttons
            Button button = isSelectPlaceHolder[i];
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }
}

