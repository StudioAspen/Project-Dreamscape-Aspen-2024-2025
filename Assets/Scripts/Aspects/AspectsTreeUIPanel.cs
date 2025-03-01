using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AspectsTreeUIPanel : UIPanel
{
    private PlayerControls playerControls;
    private GameManager gameManager;
    private AspectsManager aspectsManager;

    [Header("References")]
    [SerializeField] private AspectButtonUI aspectButtonUIPrefab;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tokensText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [SerializeField] private float aspectButtonSpacing = 250f;

    private List<AspectButtonUI> aspectButtonUIs = new List<AspectButtonUI>();

    // Use awake here because UI scene loads last
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        aspectsManager = FindObjectOfType<Player>().GetComponent<AspectsManager>();
    }

    private void Start()
    {
        closeButton.onClick.AddListener(CloseButton_OnClick);
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(CloseButton_OnClick);
    }

    private void CloseButton_OnClick()
    {
        gameManager.ChangeState(GameState.PLAYING);
    }

    private void OnEnable()
    {
        if(aspectsManager == null)
        {
            //Debug.LogWarning("Aspects manager not found");
            return;
        }

        // reset scroll rect back to center
        if (scrollRect.normalizedPosition != Vector2.zero) scrollRect.normalizedPosition = Vector2.zero;

        GenerateTree(aspectsManager.CurrentAspectTree);
    }

    private void OnDisable()
    {
        DeleteTree();
    }

    private void GenerateTree(AspectTree aspectTree)
    {
        if(aspectTree == null)
        {
            titleText.text = "Selected Aspect Tree is missing";
            return;
        }
        titleText.text = $"{aspectTree.name}";
        tokensText.text = $"Tokens: {aspectsManager.AspectTokens}";

        for(int i = 0; i < aspectTree.GetTotalLevels(); i++)
        {
            List<AspectNodeNode> aspectNodes = aspectTree.GetNodesAtLevel(i);

            for(int j = 0; j < aspectNodes.Count; j++)
            {
                AspectButtonUI aspectButtonUI = Instantiate(aspectButtonUIPrefab, contentTransform);
                aspectButtonUI.Init(gameManager, this, aspectsManager, aspectNodes[j]);

                if(aspectNodes.Count % 2 == 1) aspectButtonUI.transform.localPosition = aspectButtonSpacing * new Vector3(i, j, 0);
                else aspectButtonUI.transform.localPosition = new Vector3(aspectButtonSpacing * i, aspectButtonSpacing * j - aspectButtonSpacing / 2f, 0);
    
                aspectButtonUIs.Add(aspectButtonUI);
            }   
        }
    }

    public void UpdateTree()
    {
        tokensText.text = $"Tokens: {aspectsManager.AspectTokens}"; // Update token count

        // Update button states
        foreach (AspectButtonUI aspectButtonUI in new List<AspectButtonUI>(aspectButtonUIs))
        {
            aspectButtonUI.UpdateDisplayContents();
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
}
