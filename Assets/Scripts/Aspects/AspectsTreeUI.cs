using KBCore.Refs;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AspectsTreeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager; 
    [SerializeField] private AspectsManager aspectsManager;
    [SerializeField] private AspectButtonUI aspectButtonUIPrefab;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tokensText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform contentTransform;

    [Header("Settings")]
    [SerializeField] private float aspectButtonSpacing = 250f;

    private List<AspectButtonUI> aspectButtonUIs = new List<AspectButtonUI>();

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

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if(newState != GameState.ASPECT_SELECTION)
        {
            Disable();
            return;
        }

        Enable();
    }

    private void Start()
    {
        Disable();
    }

    private void OnEnable()
    {
        // reset scroll rect back to center
        scrollRect.normalizedPosition = Vector2.zero;

        tokensText.text = $"Tokens: {aspectsManager.AspectTokens}";

        GenerateTree();
    }
    
    private void OnDisable()
    {
        DeleteTree();
    }

    private void GenerateTree()
    {
        AspectTree aspectTree = aspectsManager.CurrentAspectTree;
        if(aspectTree == null)
        {
            titleText.text = "Missing Aspect";
            Debug.LogWarning("No aspect tree selected, press T to assign");
            return;
        }
        titleText.text = $"{aspectTree.name}";

        Vector2Int currentNodeLevel = aspectTree.GetNodeLevel(aspectTree.GetMostRecentlyAppliedNode());
        if(currentNodeLevel == new Vector2Int(-1, -1)) currentNodeLevel = new Vector2Int(0, 0);

        Vector3 rootNodePosition = new Vector3(-currentNodeLevel.x * aspectButtonSpacing, -currentNodeLevel.y * aspectButtonSpacing, 0);

        for(int i = 0; i < aspectTree.GetTotalLevels(); i++)
        {
            List<AspectNodeNode> aspectNodes = aspectTree.GetNodesAtLevel(i);

            for(int j = 0; j < aspectNodes.Count; j++)
            {
                AspectButtonUI aspectButtonUI = Instantiate(aspectButtonUIPrefab, contentTransform);
                aspectButtonUI.Init(this, aspectsManager, aspectNodes[j]);

                if(aspectNodes.Count % 2 == 1) aspectButtonUI.transform.localPosition = rootNodePosition + aspectButtonSpacing * new Vector3(i, j, 0);
                else aspectButtonUI.transform.localPosition = rootNodePosition + new Vector3(aspectButtonSpacing * i, aspectButtonSpacing * j - aspectButtonSpacing / 2f, 0);
    
                aspectButtonUIs.Add(aspectButtonUI);
            }   
        }
    }

    private void DeleteTree()
    {
        foreach (AspectButtonUI aspectButtonUI in new List<AspectButtonUI>(aspectButtonUIs))
        {
            Destroy(aspectButtonUI.gameObject);
        }

        aspectButtonUIs.Clear();
    }

    private void Enable()
    {
        gameObject.SetActive(true);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
