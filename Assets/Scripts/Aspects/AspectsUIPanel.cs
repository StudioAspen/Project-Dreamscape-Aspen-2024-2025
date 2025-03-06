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
    private AspectsManager aspectsManager;

    [Header("References")]
    [SerializeField] private AspectOptionUI[] aspectOptions = new AspectOptionUI[3];
    [SerializeField] private TMP_Text tokensText;

    public bool IsSelectingAspect { get; private set; }

    // Use awake here because UI scene loads last
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        aspectsManager = FindObjectOfType<Player>().GetComponent<AspectsManager>();
    }

    private void OnEnable()
    {
        if(aspectsManager == null)
        {
            //Debug.LogWarning("Aspects manager not found");
            return;
        }

        if(gameManager.CurrentState == GameState.ASPECT_SELECTION && aspectsManager.AspectTokens == 0)
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
            return;
        }

        AssignRandomAspectOptions();
        SetBasicOptionsNavigation();
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        if (aspectsManager == null) return;

        tokensText.text = $"{aspectsManager.AspectTokens}";
    }

    private void AssignRandomAspectOptions()
    {
        // Populate options with already equipped aspects
        int equippedCount = 0;
        for(int i = 0; i < aspectsManager.EquippedAspectTrees.Length; i++)
        {
            if (aspectsManager.EquippedAspectTrees[i] == null) continue;
            aspectOptions[equippedCount].Init(true, this, aspectsManager.EquippedAspectTrees[i], aspectsManager, gameManager, equippedCount); // Init option
            equippedCount++;
        }

        List<AspectTree> availableAspects = aspectsManager.GetAvailableNonEquippedAspects();
        // Populate remaining options with random aspects
        for (int i = equippedCount; i < aspectOptions.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableAspects.Count);
            AspectTree randomTree = availableAspects[randomIndex];
            aspectOptions[i].Init(false, this, randomTree, aspectsManager, gameManager, i);
            availableAspects.Remove(randomTree);
        }


        if(aspectsManager.AreAllEquippedAspectsCompleted()) gameManager.ChangeState(GameState.BIOME_SELECTION);
    }

    public void SelectOptionUI(int index)
    {
        IsSelectingAspect = true;
        MoveOptionToMiddle(index);
    }
    
    public void DeselectOptionUI()
    {
        IsSelectingAspect = false;

        // Update navigation
        SetBasicOptionsNavigation();
        EventSystem.current.SetSelectedGameObject(aspectOptions[1].gameObject); 
    }

    private void MoveOptionToMiddle(int index)
    {
        // Swap positions
        Vector3 oldPosition = aspectOptions[index].transform.localPosition;
        aspectOptions[index].transform.DOLocalMove(aspectOptions[1].transform.localPosition, 0.25f).SetUpdate(true);
        aspectOptions[1].transform.DOLocalMove(oldPosition, 0.25f).SetUpdate(true);

        // Swap references
        AspectOptionUI currentMiddleOption = aspectOptions[1];
        aspectOptions[1] = aspectOptions[index];
        aspectOptions[index] = currentMiddleOption;

        // Update indices
        aspectOptions[1].AssignOptionsIndex(1);
        aspectOptions[index].AssignOptionsIndex(index);

        // Update locked navigation
        aspectOptions[1].SetLockedNavigation();
    }

    private void SetBasicOptionsNavigation()
    {
        Navigation leftNavigation = aspectOptions[0].OptionsButton.navigation;
        leftNavigation.mode = Navigation.Mode.Explicit;
        leftNavigation.selectOnUp = aspectOptions[0].OptionsButton;
        leftNavigation.selectOnDown = aspectOptions[0].OptionsButton;
        leftNavigation.selectOnLeft = aspectOptions[2].OptionsButton;
        leftNavigation.selectOnRight = aspectOptions[1].OptionsButton;
        aspectOptions[0].OptionsButton.navigation = leftNavigation;

        Navigation middleNavigation = aspectOptions[1].OptionsButton.navigation;
        middleNavigation.mode = Navigation.Mode.Explicit;
        middleNavigation.selectOnUp = aspectOptions[1].OptionsButton;
        middleNavigation.selectOnDown = aspectOptions[1].OptionsButton;
        middleNavigation.selectOnLeft = aspectOptions[0].OptionsButton;
        middleNavigation.selectOnRight = aspectOptions[2].OptionsButton;
        aspectOptions[1].OptionsButton.navigation = middleNavigation;

        Navigation rightNavigation = aspectOptions[2].OptionsButton.navigation;
        rightNavigation.mode = Navigation.Mode.Explicit;
        rightNavigation.selectOnUp = aspectOptions[2].OptionsButton;
        rightNavigation.selectOnDown = aspectOptions[2].OptionsButton;
        rightNavigation.selectOnLeft = aspectOptions[1].OptionsButton;
        rightNavigation.selectOnRight = aspectOptions[0].OptionsButton;
        aspectOptions[2].OptionsButton.navigation = rightNavigation;
    }
}
