using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.EventSystems;

public class AspectsUIPanel : UIPanel
{
    private PlayerControls playerControls;
    private GameManager gameManager;
    public AspectsManager AspectsManager { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject aspectsOptionsPanelObject;
    [SerializeField] private List<AspectOptionUI> aspectOptions = new();
    [SerializeField] private AspectTreeViewerUI aspectTreeViewer;
    [SerializeField] private TMP_Text remainingTokensText;
    [SerializeField] private Button closeButton;

    // Use awake here because UI scene loads last
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        AspectsManager = FindObjectOfType<Player>().GetComponent<AspectsManager>();

        closeButton.onClick.AddListener(CloseButton_OnClick);
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(CloseButton_OnClick);
    }

    private void OnEnable()
    {
        // Aspects manager not found, go to biome selection
        if(AspectsManager == null)
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
            return;
        }

        // If the player has no aspect tokens (they didnt level), go to biome selection
        if(gameManager.CurrentState == GameState.ASPECT_SELECTION && AspectsManager.AspectTokens == 0)
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
            return;
        }

        // All equipped aspects are completed, go to biome selection
        if (AspectsManager.AreAllEquippedAspectsCompleted())
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
            return;
        }

        AssignRandomAspectOptions();
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        if (AspectsManager == null) return;

        remainingTokensText.text = $"Upgrades: {AspectsManager.AspectTokens}";
        closeButton.gameObject.SetActive(IsCloseButtonActive());

        UpdateNavigation();
    }

    private void CloseButton_OnClick()
    {
        gameManager.ChangeState(GameState.BIOME_SELECTION);
    }

    private void AssignRandomAspectOptions()
    {
        // Populate options with already equipped aspects
        int equippedCount = 0;
        for (int i = 0; i < AspectsManager.EquippedAspectTrees.Length; i++)
        {
            if (AspectsManager.EquippedAspectTrees[i] == null) continue;
            aspectOptions[equippedCount].Init(true, this, AspectsManager.EquippedAspectTrees[i]); // Init option
            equippedCount++;
        }

        List<AspectTree> availableAspects = AspectsManager.GetAvailableNonEquippedAspects();
        // Populate remaining options with random aspects
        for (int i = equippedCount; i < aspectOptions.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableAspects.Count);
            AspectTree randomTree = availableAspects[randomIndex];
            aspectOptions[i].Init(false, this, randomTree);
            availableAspects.Remove(randomTree);
        }
    }

    public void OpenAspectTreeViewer(AspectTree tree, bool isRuntimeInstance)
    {
        aspectTreeViewer.OpenTreeViewer(tree, isRuntimeInstance);
        aspectsOptionsPanelObject.SetActive(false);
    }

    public void OnAspectTreeViewerClose()
    {
        aspectsOptionsPanelObject.SetActive(true);
    }

    public void TryConvertOptionsToRuntimeInstance(AspectTree tree)
    {
        foreach(var option in aspectOptions)
        {
            option.TryConvertToRuntimeInstance(tree);
        }
    }

    private bool IsCloseButtonActive()
    {
        return AspectsManager.AspectTokens <= 0 || AspectsManager.AreAllEquippedAspectsCompleted();
    }

    private void UpdateNavigation()
    {
        Navigation leftNavigation = aspectOptions[0].OptionsButton.navigation;
        leftNavigation.mode = Navigation.Mode.Explicit;
        leftNavigation.selectOnUp = aspectOptions[0].OptionsButton;
        leftNavigation.selectOnDown = aspectOptions[0].OptionsButton;
        leftNavigation.selectOnLeft = aspectOptions[1].OptionsButton;
        leftNavigation.selectOnRight = IsCloseButtonActive() ? closeButton : aspectOptions[1].OptionsButton;
        aspectOptions[0].OptionsButton.navigation = leftNavigation;

        Navigation rightNavigation = aspectOptions[0].OptionsButton.navigation;
        rightNavigation.mode = Navigation.Mode.Explicit;
        rightNavigation.selectOnUp = aspectOptions[1].OptionsButton;
        rightNavigation.selectOnDown = aspectOptions[1].OptionsButton;
        rightNavigation.selectOnLeft = IsCloseButtonActive() ? closeButton : aspectOptions[0].OptionsButton;
        rightNavigation.selectOnRight = aspectOptions[0].OptionsButton;
        aspectOptions[1].OptionsButton.navigation = rightNavigation;
    }
}
