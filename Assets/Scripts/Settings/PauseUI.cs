using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
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

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        resumeButton.OnButtonClicked += ResumeButton_OnButtonClicked;
        optionsButton.OnButtonClicked += OptionsButton_OnButtonClicked;
        saveButton.OnButtonClicked += SaveButton_OnButtonClicked;
        menuButton.OnButtonClicked += MenuButton_OnButtonClicked;
        quitButton.OnButtonClicked += QuitButton_OnButtonClicked;

        confirmQuitButton.OnButtonClicked += ConfirmQuitButton_OnButtonClicked;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;

        resumeButton.OnButtonClicked -= ResumeButton_OnButtonClicked;
        optionsButton.OnButtonClicked -= OptionsButton_OnButtonClicked;
        saveButton.OnButtonClicked -= SaveButton_OnButtonClicked;
        menuButton.OnButtonClicked -= MenuButton_OnButtonClicked;
        quitButton.OnButtonClicked -= QuitButton_OnButtonClicked;

        confirmQuitButton.OnButtonClicked -= ConfirmQuitButton_OnButtonClicked;
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
        throw new NotImplementedException();
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
