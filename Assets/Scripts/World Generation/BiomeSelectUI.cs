using DG.Tweening;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiomeSelectUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Self] private Image panel;
    [SerializeField] private List<BiomeCardUI> biomeCards;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        Disable();
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.BIOME_SELECTION)
        {
            Disable();
            return;
        }

        Enable();
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        AssignRandomBiomesToCards();

        EnableCards();
    }

    public void Disable()
    {
        gameObject.SetActive(false);

        DisableCards();
    }

    private void AssignRandomBiomesToCards()
    {
        foreach(BiomeCardUI card in biomeCards)
        {
            Biome randomBiome = (Biome)Random.Range(0, System.Enum.GetValues(typeof(Biome)).Length);

            card.AssignCardBiome(randomBiome);
        }
    }

    private void EnableCards()
    {
        foreach (BiomeCardUI card in biomeCards)
        {
            card.EnableButton();
        }
    }

    private void DisableCards()
    {
        foreach (BiomeCardUI card in biomeCards)
        {
            card.DisableButton();
        }
    }
}
