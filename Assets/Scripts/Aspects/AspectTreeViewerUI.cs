using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AspectTreeViewerUI : MonoBehaviour
{
    private bool isRuntimeInstance;
    private AspectsUIPanel aspectsUIPanel;
    private AspectTree currentTree;

    [SerializeField] private GameObject defaultSelectedObject;

    [Header("References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text upgradesText;
    private Vector3 upgradesTextOriginalLocalPosition;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text closeButtonText;

    [Header("Node Content References")]
    [SerializeField] private Image nodeContentObjectImage;
    [SerializeField] private TMP_Text nodeContentTitleText;
    [SerializeField] private TMP_Text nodeContentDescriptionText;
    [SerializeField] private TMP_Text lockedStatusText;
    private Vector3 lockedStatusTextOriginalLocalPosition;

    [Header("Node References")]
    [SerializeField] private Image treeLinesImage;
    [SerializeField] private List<Button> nodesButtons = new(); // Length = 7, Indices 1-2 are the split.
    [SerializeField] private Sprite diamondGlowSprite;
    private Sprite originalDiamondSprite;

    private void Awake()
    {
        aspectsUIPanel = GetComponentInParent<AspectsUIPanel>();

        upgradesTextOriginalLocalPosition = upgradesText.transform.localPosition;
        lockedStatusTextOriginalLocalPosition = lockedStatusText.transform.localPosition;
        originalDiamondSprite = nodesButtons[0].image.sprite;

        closeButton.onClick.AddListener(CloseButton_OnClick);
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(CloseButton_OnClick);
    }

    private void OnEnable()
    {
        aspectsUIPanel.ChangeDefaultSelectedObject(defaultSelectedObject);
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
    }

    private void OnDisable()
    {
        aspectsUIPanel.RestoreDefaultSelectedObject();
        EventSystem.current.SetSelectedGameObject(aspectsUIPanel.DefaultSelectedObject);

        aspectsUIPanel.OnAspectTreeViewerClose();
    }

    public void OpenTreeViewer(AspectTree tree, bool isRuntimeInstance)
    {
        gameObject.SetActive(true);

        currentTree = tree;
        this.isRuntimeInstance = isRuntimeInstance;
        UpdateWithTreeData(currentTree);

        OnNodeButtonSelected(0);
    }

    private void CloseButton_OnClick()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called through the button event in the inspector.
    /// </summary>
    public void OnNodeButtonClicked(int index)
    {
        AspectNodeNode node = currentTree.GetNodeAtIndex(index);

        // Convert to runtime instance if first node
        if (index == 0 && !isRuntimeInstance)
        {
            currentTree = aspectsUIPanel.AspectsManager.AddAspectTree(currentTree);
            aspectsUIPanel.TryConvertOptionsToRuntimeInstance(currentTree);
            isRuntimeInstance = true;

            node = currentTree.GetNodeAtIndex(index);
        }

        if (!currentTree.GetNextUnappliedNodes().Contains(node))
        {
            ShakeLockText();
            return;
        }

        if(aspectsUIPanel.AspectsManager.AspectTokens <= 0)
        {
            ShakeUpgradesText();
            return;
        }

        // Apply the node
        node.ApplyAspect(aspectsUIPanel.AspectsManager);
        aspectsUIPanel.AspectsManager.ConsumeAspectToken();

        // Update visuals
        UpdateWithTreeData(currentTree);
        UpdateNodeContent(node);

        ShakeUpgradesText();
    }

    /// <summary>
    /// Called through the event trigger in the inspector.
    /// </summary>
    public void OnNodeButtonSelected(int index)
    {
        nodesButtons[index].image.sprite = diamondGlowSprite;

        AspectNodeNode node = currentTree.GetNodeAtIndex(index);

        UpdateNodeContent(node);
        // If last layer, hide the potential upgrade if a split-level node has not been unlocked
        if (index == 6 && currentTree.GetNextUnappliedNodes().Count > 0 && currentTree.GetNodeLevel(currentTree.GetNextUnappliedNodes()[0]).x < 2)
        {
            nodeContentTitleText.text = $"Hidden Passive Upgrade";
            nodeContentDescriptionText.text = $"Unlock a split-level node to reveal this node!";
        }
        nodeContentObjectImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called through the event trigger in the inspector.
    /// </summary>
    public void OnNodeButtonDeselected(int index)
    {
        nodesButtons[index].image.sprite = originalDiamondSprite;

        nodeContentObjectImage.gameObject.SetActive(false);
    }

    private void UpdateNodeContent(AspectNodeNode node)
    {
        lockedStatusText.transform.localPosition = lockedStatusTextOriginalLocalPosition;
        lockedStatusText.transform.DOKill();

        nodeContentTitleText.text = node.DisplayName;
        nodeContentDescriptionText.text = node.Description;

        if (node.IsApplied)
        {
            lockedStatusText.text = $"APPLIED";
            return;
        }

        if (currentTree.GetNextUnappliedNodes().Contains(node))
        {
            lockedStatusText.text = $"AVAILABLE";
        }
        else
        {
            lockedStatusText.text = $"LOCKED";
        }
    } 

    private void UpdateWithTreeData(AspectTree tree)
    {
        SetColor(tree.AspectTextColor);

        titleText.text = tree.DisplayName;
        upgradesText.text = $"Upgrades: {aspectsUIPanel.AspectsManager.AspectTokens}";

        for (int i = 0; i < nodesButtons.Count; i++)
        {
            AspectNodeNode node = tree.GetNodeAtIndex(i);
            if(node == null)
            {
                SetTransparency(nodesButtons[i].image, 0.25f);
                continue;
            }
            if(tree.GetNextUnappliedNodes().Contains(node))
            {
                SetTransparency(nodesButtons[i].image, 1f);
                continue;
            }
            if (node.IsApplied)
            {
                SetTransparency(nodesButtons[i].image, 0.5f);
                continue;
            }
            SetTransparency(nodesButtons[i].image, 0.25f);
        }
    }

    private void SetColor(Color color)
    {
        titleText.color = color;
        closeButtonText.color = color;
        upgradesText.color = color;

        nodeContentObjectImage.color = color;
        nodeContentTitleText.color = color;
        nodeContentDescriptionText.color = color;
        lockedStatusText.color = color;

        treeLinesImage.color = color;
        foreach (Button button in nodesButtons) button.image.color = color;
    }

    private void SetTransparency(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private void ShakeLockText()
    {
        lockedStatusText.transform.DOKill();
        lockedStatusText.transform.localPosition = lockedStatusTextOriginalLocalPosition;

        float duration = 0.5f;
        float strength = 10f;

        lockedStatusText.transform.DOShakePosition(duration, strength).SetUpdate(true);
    }

    private void ShakeUpgradesText()
    {
        upgradesText.transform.DOKill();
        upgradesText.transform.localPosition = upgradesTextOriginalLocalPosition;

        float duration = 0.5f;
        float strength = 10f;

        upgradesText.transform.DOShakePosition(duration, strength).SetUpdate(true);
    }
}
