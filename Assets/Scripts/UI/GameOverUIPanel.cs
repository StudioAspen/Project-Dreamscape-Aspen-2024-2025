using Eflatun.SceneReference;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIPanel : UIPanel
{
    [Header("Scene")]
    [SerializeField] private SceneReference menuScene;

    [Header("Buttons")]
    [SerializeField] private Button menuButton;

    private void Awake()
    {
        menuButton.onClick.AddListener(MenuButton_OnClicked);
    }

    private void OnDestroy()
    {
        menuButton.onClick.RemoveListener(MenuButton_OnClicked);
    }

    private void MenuButton_OnClicked()
    {
        SceneManager.LoadScene(menuScene.Name);
    }
}