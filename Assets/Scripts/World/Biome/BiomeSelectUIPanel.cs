using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BiomeSelectUIPanel : UIPanel
{
    [Header("References")]
    [SerializeField] private List<BiomeCardUI> biomeCards;

    private void Start()
    {
        
    }

    public void OnEnable()
    {
        AssignRandomBiomesToCards();

        EnableCards();
    }

    public void OnDisable()
    {
        DisableCards();
    }

    private void AssignRandomBiomesToCards()
    {
        List<Biome> potentialBiomes = System.Enum.GetValues(typeof(Biome)).Cast<Biome>().ToList();

        foreach (BiomeCardUI card in biomeCards)
        {
            int randomIndex = UnityEngine.Random.Range(0, potentialBiomes.Count);

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
