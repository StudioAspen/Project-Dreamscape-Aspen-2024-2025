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

public class BiomeSelectUI : MonoBehaviour
{
    private InputManager inputManager;
    private GameManager gameManager;
    private Image panel;

    [Header("References")]
    [SerializeField] private List<BiomeCardUI> biomeCards;

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        gameManager = FindObjectOfType<GameManager>();
        panel = GetComponent<Image>();

        inputManager.OnControlSchemeChanged += InputManager_OnControlSchemeChanged;

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        inputManager.OnControlSchemeChanged -= InputManager_OnControlSchemeChanged;

        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void InputManager_OnControlSchemeChanged(InputManager.ControlScheme newControlScheme)
    {
        if (gameManager.CurrentState != GameState.BIOME_SELECTION) return;

        // Visually deselect all cards
        foreach (BiomeCardUI card in biomeCards)
        {
            //card.DisableSelectedIndicator();
        }

        if (newControlScheme == InputManager.ControlScheme.GAMEPAD)
        {
            // Set the middle card as selected
            EventSystem.current.SetSelectedGameObject(biomeCards[1].gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
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

        if (inputManager.CurrentControlScheme == InputManager.ControlScheme.GAMEPAD) EventSystem.current.SetSelectedGameObject(biomeCards[1].gameObject);
    }

    private void DisableCards()
    {
        foreach (BiomeCardUI card in biomeCards)
        {
            card.DisableButton();
        }
    }
}
