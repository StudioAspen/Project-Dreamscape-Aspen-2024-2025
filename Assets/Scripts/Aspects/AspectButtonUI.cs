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

    /// <summary>
    /// Initializes the AspectButtonUI with the given parameters.
    /// </summary>
    /// <param name="treeUI">The AspectsTreeUI instance.</param>
    /// <param name="aspectsManager">The AspectsManager instance.</param>
    /// <param name="aspectNode">The AspectNodeNode instance.</param>
    public void Init(AspectsTreeUI treeUI, AspectsManager aspectsManager, AspectNodeNode aspectNode)
    {
        aspectsTreeUI = treeUI;
        this.aspectsManager = aspectsManager;
        this.aspectNode = aspectNode;

        ChangeButton(treeUI, aspectsManager, aspectNode);
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

    /// <summary>
    /// Changes the appearance and behavior of the button based on the given aspect node.
    /// </summary>
    /// <param name="treeUI">The AspectsTreeUI instance.</param>
    /// <param name="aspectsManager">The AspectsManager instance.</param>
    /// <param name="aspectNode">The AspectNodeNode instance.</param>
    private void ChangeButton(AspectsTreeUI treeUI, AspectsManager aspectsManager, AspectNodeNode aspectNode)
    {
        // only the next possible nodes will be interactable and on the same level as the first applied multi-node level node
        button.interactable = aspectsManager.CurrentAspectTree.GetNextUnappliedNodes().Contains(aspectNode) && aspectsManager.AspectTokens > 0 && aspectsManager.CurrentAspectTree.CanMultiNodeLevelNodeBeChosen(aspectNode);
        // make the button green if its been applied already
        GetComponent<Image>().color = aspectNode.IsApplied ? Color.green : GetComponent<Image>().color;

        if (aspectNode.GetType() == typeof(ComboAspectNodeNode))
        {
            ComboAspectNodeNode comboAspectNode = aspectNode as ComboAspectNodeNode;

            text.text = comboAspectNode == null ? $"Empty combo" : $"{comboAspectNode.ComboData.name}";
        }

        if (aspectNode.GetType() == typeof(StatusEffectAspectNodeNode))
        {
            StatusEffectAspectNodeNode statusEffectAspectNode = aspectNode as StatusEffectAspectNodeNode;

            text.text = statusEffectAspectNode.StatusEffect == null ? $"Empty status effect" : $"{statusEffectAspectNode.StatusEffect.name}";
        }
    }
}
