using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInManager : MonoBehaviour
{
    private GameInputManager gameInputManager;
    
    [Header("References")]
    [SerializeField] private Image fadeImage;

    [Header("Settings")]
    [SerializeField] private float fadeInTime = 2f;

    private void Awake()
    {
        fadeImage.color = Color.black;

        LoadingScreenManager.OnLoadingScreenFinished += LoadingScreenManager_OnLoadingScreenFinished;
    }

    private void Start()
    {
        gameInputManager = FindObjectOfType<GameInputManager>();
        gameInputManager.EnableControls(false);
    }

    private void OnDestroy()
    {
        LoadingScreenManager.OnLoadingScreenFinished -= LoadingScreenManager_OnLoadingScreenFinished;
    }

    private void LoadingScreenManager_OnLoadingScreenFinished()
    {
        fadeImage.DOColor(Color.clear, fadeInTime).SetUpdate(true).OnComplete(() =>
        {
            gameInputManager.EnableControls(true);
            Destroy(gameObject);
        });
    }
}
