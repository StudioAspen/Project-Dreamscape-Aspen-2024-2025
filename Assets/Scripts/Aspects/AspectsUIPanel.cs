using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AspectsUIPanel : UIPanel
{
    private PlayerControls playerControls;
    private GameManager gameManager;
    private AspectsManager aspectsManager;

    [Header("References")]
    [SerializeField] private AspectOptionUI[] aspectOptions = new AspectOptionUI[3];
    [SerializeField] private TMP_Text tokensText;

    public bool IsSelectingBranch { get; private set; }

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
        List<AspectTree> availableAspects = new List<AspectTree>(aspectsManager.AllAspectTrees);

        for(int i = 0; i < aspectOptions.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availableAspects.Count);
            AspectTree randomTree = availableAspects[randomIndex];
            aspectOptions[i].Init(this, randomTree, aspectsManager, gameManager, i);
            availableAspects.Remove(randomTree);
        }
    }

    public void SelectBranchOptionUI(int index)
    {
        IsSelectingBranch = true;

        MoveOptionToMiddle(index);
    }
    
    public void DeselectBranchOptionUI()
    {
        IsSelectingBranch = false;
    }

    private void MoveOptionToMiddle(int index)
    {
        // Swap positions
        Vector3 optionPosition = aspectOptions[index].transform.localPosition;
        aspectOptions[index].transform.localPosition = aspectOptions[1].transform.localPosition;
        aspectOptions[1].transform.localPosition = optionPosition;

        // Swap references
        AspectOptionUI currentMiddleOption = aspectOptions[1];
        aspectOptions[1] = aspectOptions[index];
        aspectOptions[index] = currentMiddleOption;

        // Update indices
        aspectOptions[1].AssignOptionsIndex(1);
        aspectOptions[index].AssignOptionsIndex(index);
    }
}
