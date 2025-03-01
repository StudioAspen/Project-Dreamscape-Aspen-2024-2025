using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseUIPanel : UIPanel
{
    private GameManager gameManager;

    [Header("Pause Menu Windows")]
    [SerializeField] private GameObject pauseMenuObject;
    [SerializeField] private GameObject optionsMenuObject;
    
    
    [Header("Pause Menu Buttons")]
    [SerializeField] private PauseButtonUI resumeButton;
    [SerializeField] private PauseButtonUI optionsButton;
    [SerializeField] private PauseButtonUI saveButton;
    [SerializeField] private PauseButtonUI menuButton;
    [SerializeField] private PauseButtonUI quitButton;

    [Header("Options Menu Buttons")]
    [SerializeField] private PauseButtonUI vSyncButton;
    [SerializeField] private PauseButtonUI optionsConfirmButton;
    // Need a master volume slider//
    

    [Header("Quit Confirmation")]
    [SerializeField] private GameObject confirmQuitObject;
    [SerializeField] private PauseButtonUI confirmQuitButton;

    [field: Header("Button Highlight Color")]
    [field: SerializeField] public Color ButtonHighlightColor { get; private set; }
    
    [field: Header("Slider Colors")]
    [field: SerializeField] public Color SliderHighlightColor { get; private set; }
    [field: SerializeField] public Color SliderOriginalColor { get; private set; }
    
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        resumeButton.OnButtonClicked += ResumeButton_OnButtonClicked;
        optionsButton.OnButtonClicked += OptionsButton_OnButtonClicked;
        saveButton.OnButtonClicked += SaveButton_OnButtonClicked;
        menuButton.OnButtonClicked += MenuButton_OnButtonClicked;
        quitButton.OnButtonClicked += QuitButton_OnButtonClicked;
        
        vSyncButton.OnButtonClicked += VSyncButton_OnButtonClicked;
        optionsConfirmButton.OnButtonClicked += OptionsConfirmButton_OnButtonClicked;

        //confirmQuitButton.OnButtonClicked += ConfirmQuitButton_OnButtonClicked;
        
        // Check VSync Value
        vSyncButton.GetComponentInChildren<TMP_Text>().text = "VSync: " + (QualitySettings.vSyncCount == 1 ? "ON" : "OFF");
    }

    private void OnDestroy()
    {
        resumeButton.OnButtonClicked -= ResumeButton_OnButtonClicked;
        optionsButton.OnButtonClicked -= OptionsButton_OnButtonClicked;
        saveButton.OnButtonClicked -= SaveButton_OnButtonClicked;
        menuButton.OnButtonClicked -= MenuButton_OnButtonClicked;
        quitButton.OnButtonClicked -= QuitButton_OnButtonClicked;

        //confirmQuitButton.OnButtonClicked -= ConfirmQuitButton_OnButtonClicked;
    }

    #region Pause Menu Button Click Events
    private void ResumeButton_OnButtonClicked()
    {
        gameManager.ChangeState(gameManager.PreviousState);
    }

    private void OptionsButton_OnButtonClicked()
    {
        pauseMenuObject.SetActive(false);
        optionsMenuObject.SetActive(true);
    }

    private void SaveButton_OnButtonClicked()
    {
        Debug.LogWarning("Save not implemented yet.");
    }

    private void MenuButton_OnButtonClicked()
    {
        gameManager.GoBackToMenu();
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
    
    
    #region Options Menu Button Click Events

    private void VSyncButton_OnButtonClicked() {
        QualitySettings.vSyncCount = QualitySettings.vSyncCount == 0 ? 1 : 0;
        TMP_Text textField = vSyncButton.GetComponentInChildren<TMP_Text>();
        textField.text = "VSync: " + (QualitySettings.vSyncCount == 1 ? "ON" : "OFF");
        vSyncButton.GetComponentInChildren<PauseButtonUI>().SetOriginalText(textField.text);
        textField.text = ">" + textField.text + "<";
    }

    private void OptionsConfirmButton_OnButtonClicked() {
        pauseMenuObject.SetActive(true);
        optionsMenuObject.SetActive(false);
    }
    
    
    
    
    #endregion
    
    
    
    
    
    
    
    
}
