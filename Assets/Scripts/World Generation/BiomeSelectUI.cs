using DG.Tweening;
using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiomeSelectUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Self] private Image panel;
    [SerializeField] private List<BiomeCardUI> biomeCards;

    [SerializeField, Scene] EventManager eventManager;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        Disable();
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
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

        eventManager.setEventClearStatus(false);
    }

    public void Disable()
    {
        gameObject.SetActive(false);

        DisableCards();
    }

    private void AssignRandomBiomesToCards()
    {
        List<Biome> potentialBiomes = System.Enum.GetValues(typeof(Biome)).Cast<Biome>().ToList();

        foreach (BiomeCardUI card in biomeCards)
        {
            int randomIndex = Random.Range(0, potentialBiomes.Count);

            Biome randomBiome = potentialBiomes[randomIndex];

            card.AssignCardBiome(randomBiome);

            potentialBiomes.RemoveAt(randomIndex);
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
