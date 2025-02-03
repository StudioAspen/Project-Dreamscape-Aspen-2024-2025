using DG.Tweening.Core.Easing;
using Eflatun.SceneReference;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIPanel : UIPanel
{
    private GameManager gameManager;

    [Header("Scene")]
    [SerializeField] private SceneReference menuScene;

    [Header("Buttons")]
    [SerializeField] private Button menuButton;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        menuButton.onClick.AddListener(MenuButton_OnClicked);
    }

    private void OnDestroy()
    {
        menuButton.onClick.RemoveListener(MenuButton_OnClicked);
    }

    private void MenuButton_OnClicked()
    {
        gameManager.SetTimeScale(1f); // Reset the time scale so that in the next scene, the game runs at normal speed
        SceneManager.LoadScene(menuScene.Name, LoadSceneMode.Single);
    }
}