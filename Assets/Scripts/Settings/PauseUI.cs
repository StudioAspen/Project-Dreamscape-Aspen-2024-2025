using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseUI : MonoBehaviour
{
    private InputManager playerInputManager;
    private PlayerControls playerControls;
    private GameManager gameManager;

    [Header("Pause Buttons")]
    [SerializeField] private PauseButtonUI resumeButton;
    [SerializeField] private PauseButtonUI optionsButton;
    [SerializeField] private PauseButtonUI saveButton;
    [SerializeField] private PauseButtonUI menuButton;
    [SerializeField] private PauseButtonUI quitButton;

    [Header("Quit Confirmation")]
    [SerializeField] private GameObject confirmQuitObject;
    [SerializeField] private PauseButtonUI confirmQuitButton;

    [field: Header("Button Highlight Color")]
    [field: SerializeField] public Color ButtonHighlightColor { get; private set; }

    private void Awake()
    {
        playerInputManager = FindObjectOfType<InputManager>();
        playerControls = playerInputManager.PlayerControls;
        gameManager = FindObjectOfType<GameManager>();

        playerInputManager.OnControlSchemeChanged += PlayerInputManager_OnControlSchemeChanged;

        playerControls.Gameplay.Pause.performed += PlayerControls_OnPausePerformed;

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        resumeButton.OnButtonClicked += ResumeButton_OnButtonClicked;
        optionsButton.OnButtonClicked += OptionsButton_OnButtonClicked;
        saveButton.OnButtonClicked += SaveButton_OnButtonClicked;
        menuButton.OnButtonClicked += MenuButton_OnButtonClicked;
        quitButton.OnButtonClicked += QuitButton_OnButtonClicked;

        //confirmQuitButton.OnButtonClicked += ConfirmQuitButton_OnButtonClicked;
    }

    private void OnDestroy()
    {
        playerInputManager.OnControlSchemeChanged -= PlayerInputManager_OnControlSchemeChanged;

        playerControls.Gameplay.Pause.performed -= PlayerControls_OnPausePerformed;

        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;

        resumeButton.OnButtonClicked -= ResumeButton_OnButtonClicked;
        optionsButton.OnButtonClicked -= OptionsButton_OnButtonClicked;
        saveButton.OnButtonClicked -= SaveButton_OnButtonClicked;
        menuButton.OnButtonClicked -= MenuButton_OnButtonClicked;
        quitButton.OnButtonClicked -= QuitButton_OnButtonClicked;

        //confirmQuitButton.OnButtonClicked -= ConfirmQuitButton_OnButtonClicked;
    }

    private void PlayerInputManager_OnControlSchemeChanged(InputManager.ControlScheme newControlScheme)
    {
        if (gameManager.CurrentState != GameState.PAUSED) return;

        if (newControlScheme == InputManager.ControlScheme.GAMEPAD)
        {
            // Set the resume button as selected
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void PlayerControls_OnPausePerformed(InputAction.CallbackContext context)
    {
        gameManager.ChangeState(GameState.PAUSED);
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.PAUSED)
        {
            Disable();
            return;
        }

        Enable();
    }

    private void Enable()
    {
        gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    #region Button Click Events
    private void ResumeButton_OnButtonClicked()
    {
        gameManager.ChangeState(gameManager.PreviousState);
    }

    private void OptionsButton_OnButtonClicked()
    {
        Debug.LogWarning("Options not implemented yet.");
    }

    private void SaveButton_OnButtonClicked()
    {
        Debug.LogWarning("Save not implemented yet.");
    }

    private void MenuButton_OnButtonClicked()
    {
        Debug.LogWarning("Menu not implemented yet.");
    }

    private void QuitButton_OnButtonClicked()
    {
        //Debug.LogWarning("Quit not implemented yet.");
        ConfirmQuitButton_OnButtonClicked();
    }

    private void ConfirmQuitButton_OnButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
             Application.Quit(); 
#endif
    }
    #endregion
}
