using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AspectButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Scene] private GameManager gameManager;
    [SerializeField, Self] private Button button;
    [SerializeField, Child] private TMP_Text text;

    private AspectsTreeUI aspectsTreeUI;
    private AspectsManager aspectsManager;
    private AspectNodeNode aspectNode;

    public void Init(AspectsTreeUI treeUI, AspectsManager aspectsManager, AspectNodeNode aspectNode)
    {
        aspectsTreeUI = treeUI;
        this.aspectsManager = aspectsManager;
        this.aspectNode = aspectNode;

        button.interactable = aspectsManager.CurrentAspectTree.GetNextUnappliedNodes().Contains(aspectNode) && aspectsManager.AspectTokens > 0;
        GetComponent<Image>().color = aspectNode.IsApplied ? Color.green : GetComponent<Image>().color;

        if (aspectNode.GetType() == typeof(ComboAspectNodeNode)) text.text = $"Unlock combo: {(aspectNode as ComboAspectNodeNode).ComboData.name}";
        if (aspectNode.GetType() == typeof(AugmentAspectNodeNode)) text.text = $"Unlock unique passive: {(aspectNode as AugmentAspectNodeNode).name}";
        if (aspectNode.GetType() == typeof(BuffAspectNodeNode)) text.text = $"Unlock buff: {(aspectNode as BuffAspectNodeNode).name}";
    }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void Awake()
    {
        button.onClick.AddListener(OnClickButton);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClickButton);
    }

    public void OnClickButton()
    {
        aspectNode.ApplyAspect(aspectsManager);

        aspectsManager.ConsumeAspectToken();

        gameManager.ChangeState(GameState.PLAYING);
    }
}
