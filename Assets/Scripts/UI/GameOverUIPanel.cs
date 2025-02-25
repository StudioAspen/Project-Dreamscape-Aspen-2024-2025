using DG.Tweening.Core.Easing;
using Eflatun.SceneReference;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIPanel : UIPanel
{
    private GameManager gameManager;

    [Header("Buttons")]
    [SerializeField] private Button menuButton;

    private void Start()
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
        gameManager.GoBackToMenu();
    }
}