using DG.Tweening.Core.Easing;
using Eflatun.SceneReference;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static InputManager;

public class TitleScreenManager : MonoBehaviour
{
    private InputManager inputManager;

    [Header("Scenes")]
    [SerializeField] private SceneReference loadingScene;

    [Header("UI")]
    [SerializeField] private Button playButton;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();

        inputManager.OnControlSchemeChanged += InputManager_OnControlSchemeChanged;

        playButton.onClick.AddListener(PlayButton_OnClicked);
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
        SceneManager.LoadScene(loadingScene.Name);
    }
}
