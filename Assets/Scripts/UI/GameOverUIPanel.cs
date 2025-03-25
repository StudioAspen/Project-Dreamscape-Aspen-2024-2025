using DG.Tweening.Core.Easing;
using Eflatun.SceneReference;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameOverUIPanel : UIPanel
{
    private GameManager gameManager;

    [Header("References")]
    [SerializeField] private Button menuButton;
    [SerializeField] private VideoPlayer videoPlayer;

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

    private void OnEnable()
    {
        videoPlayer.Play();
    }

    private void OnDisable()
    {
        
    }

    private void MenuButton_OnClicked()
    {
        gameManager.GoBackToMenu();
    }
}