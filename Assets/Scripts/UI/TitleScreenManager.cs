using DG.Tweening.Core.Easing;
using Eflatun.SceneReference;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    private InputManager inputManager;

    [Header("Scenes")]
    [SerializeField] private SceneReference bedroomScene;

    [Header("UI")]
    [SerializeField] private Button playButton;

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();

        inputManager.OnControlSchemeChanged += InputManager_OnControlSchemeChanged;

        playButton.onClick.AddListener(PlayButton_OnClicked);

        AkSoundEngine.PostEvent("Play_TopLevelMusicContainer", gameObject);
        AkSoundEngine.SetState("GameMode", "Title");
    }

    private void OnDestroy()
    {
        inputManager.OnControlSchemeChanged -= InputManager_OnControlSchemeChanged;

        playButton.onClick.RemoveListener(PlayButton_OnClicked);
    }

    private void InputManager_OnControlSchemeChanged(InputManager.ControlScheme newControlScheme)
    {
        if (newControlScheme == InputManager.ControlScheme.GAMEPAD)
        {
            inputManager.LockCursor();
            // Set the play button as selected
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }
        else
        {
            inputManager.UnlockCursor();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void PlayButton_OnClicked()
    {
        SceneManager.LoadScene(bedroomScene.Name);
    }
}
