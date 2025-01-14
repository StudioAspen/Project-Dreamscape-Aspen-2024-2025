using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiomeSelectUI : MonoBehaviour
{
    private GameManager gameManager;
    private Image panel;

    [Header("References")]
    [SerializeField] private List<BiomeCardUI> biomeCards;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        panel = GetComponent<Image>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
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
    }

    public void Disable()
    {
        DisableCards();

        gameObject.SetActive(false);
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
