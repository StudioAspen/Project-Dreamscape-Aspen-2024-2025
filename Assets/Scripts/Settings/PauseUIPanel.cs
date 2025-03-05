using System;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private PauseButtonUI qualityPresetButton;
    [SerializeField] private PauseButtonUI optionsConfirmButton;

    [Header("Options Sliders")] 
    [SerializeField] private SliderUI volumeSlider;
    [SerializeField] private SliderUI cameraSensitivitySlider;

    [Header("Quit Confirmation")]
    [SerializeField] private GameObject confirmQuitObject;
    [SerializeField] private PauseButtonUI confirmQuitButton;

    [field: Header("Button Highlight Color")]
    [field: SerializeField] public Color ButtonHighlightColor { get; private set; }
    
    [field: Header("Slider Colors")]
    [field: SerializeField] public Color SliderHighlightColor { get; private set; }
    [field: SerializeField] public Color SliderOriginalColor { get; private set; }


    private Slider volumeSliderComponent, cameraSensitivitySliderComponent;
    private TMP_Text vSyncButtonTextComponent, qualityPresetButtonTextComponent;
    
    private void Awake() {
        optionsMenuObject.SetActive(true);
        // Store references to components needed in options panel
        InitializeInactiveComponents();
        optionsMenuObject.SetActive(false);
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        resumeButton.OnButtonClicked += ResumeButton_OnButtonClicked;
        optionsButton.OnButtonClicked += OptionsButton_OnButtonClicked;
        saveButton.OnButtonClicked += SaveButton_OnButtonClicked;
        menuButton.OnButtonClicked += MenuButton_OnButtonClicked;
        quitButton.OnButtonClicked += QuitButton_OnButtonClicked;
        
        vSyncButton.OnButtonClicked += VSyncButton_OnButtonClicked;
        qualityPresetButton.OnButtonClicked += QualityPresetButton_OnButtonClicked;
        optionsConfirmButton.OnButtonClicked += OptionsConfirmButton_OnButtonClicked;

        //confirmQuitButton.OnButtonClicked += ConfirmQuitButton_OnButtonClicked;
    }

    private void OnEnable() {
        // Update setting values every time after opening options
        volumeSliderComponent.onValueChanged.AddListener(OnVolumeSliderValueChanged);
        volumeSliderComponent.value = PlayerPreferences.Instance.MasterVolume;
        volumeSlider.SetSliderOriginalText($"Volume: {Mathf.RoundToInt(PlayerPreferences.Instance.MasterVolume * 100f)}%");
        volumeSlider.ForceUpdateText();
        
        cameraSensitivitySliderComponent.onValueChanged.AddListener(OnCameraSensitivitySliderValueChanged);
        cameraSensitivitySliderComponent.value = PlayerPreferences.Instance.CameraSensitivity;
        cameraSensitivitySlider.SetSliderOriginalText($"Sensitivity: {(float)Math.Round(PlayerPreferences.Instance.CameraSensitivity, 2)}");
        cameraSensitivitySlider.ForceUpdateText();
        
        vSyncButtonTextComponent.text = $"VSync: {(PlayerPreferences.Instance.IsVSync ? "ON" : "OFF")}";
        vSyncButton.SetOriginalText(vSyncButtonTextComponent.text);
        
        qualityPresetButtonTextComponent.text = $"Quality Level: {PlayerPreferences.Instance.GetQualityLevelDisplay()}";
        qualityPresetButton.SetOriginalText(qualityPresetButtonTextComponent.text); 
    }

    private void InitializeInactiveComponents() {
        volumeSliderComponent = volumeSlider.GetComponent<Slider>();
        cameraSensitivitySliderComponent = cameraSensitivitySlider.GetComponent<Slider>();
        vSyncButtonTextComponent = vSyncButton.GetComponentInChildren<TMP_Text>();
        qualityPresetButtonTextComponent = qualityPresetButton.GetComponentInChildren<TMP_Text>();
    }

    private void OnDisable() {
        volumeSlider.GetComponent<Slider>().onValueChanged.RemoveListener(OnVolumeSliderValueChanged);
        cameraSensitivitySlider.GetComponent<Slider>().onValueChanged.RemoveListener(OnCameraSensitivitySliderValueChanged);
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
        PlayerPreferences.Instance.SetVSync(!PlayerPreferences.Instance.IsVSync);
        TMP_Text textField = vSyncButton.GetComponentInChildren<TMP_Text>();
        textField.text = $"VSync: {(PlayerPreferences.Instance.IsVSync ? "ON" : "OFF")}";
        vSyncButton.GetComponentInChildren<PauseButtonUI>().SetOriginalText(textField.text);
        textField.text = $">{textField.text}<";
    }

    private void OptionsConfirmButton_OnButtonClicked() {
        pauseMenuObject.SetActive(true);
        optionsMenuObject.SetActive(false);
    }

    private void OnVolumeSliderValueChanged(float newValue) {
        PlayerPreferences.Instance.SetMasterVolume(newValue);
        volumeSlider.SetSliderOriginalText($"Volume: {Mathf.RoundToInt(newValue * 100f)}%");
        volumeSlider.ForceUpdateText();
    }

    private void OnCameraSensitivitySliderValueChanged(float newValue) {
        PlayerPreferences.Instance.SetCameraSensitivity(newValue);
        cameraSensitivitySlider.SetSliderOriginalText($"Sensitivity: {(float)Math.Round(newValue, 2)}");
        cameraSensitivitySlider.ForceUpdateText();
    }

    private void QualityPresetButton_OnButtonClicked() {
        PlayerPreferences.Instance.SetQualityLevel((PlayerPreferences.Instance.QualityLevel + 1) % 5);
        TMP_Text textField = qualityPresetButton.GetComponentInChildren<TMP_Text>();
        textField.text = $"Quality Level: {PlayerPreferences.Instance.GetQualityLevelDisplay()}";
        qualityPresetButton.GetComponentInChildren<PauseButtonUI>().SetOriginalText(textField.text);
        textField.text = ">" + textField.text + "<";
    }
    
    
    #endregion
    
    
    
    
    
}
