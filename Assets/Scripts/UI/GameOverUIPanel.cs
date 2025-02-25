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

    // UI scene loads last so Awake is safe here
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
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