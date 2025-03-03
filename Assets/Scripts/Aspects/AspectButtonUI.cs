using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AspectButtonUI : MonoBehaviour
{
    private GameManager gameManager;
    private AspectsUIPanel aspectsTreeUI;
    private AspectsManager aspectsManager;
    private AspectNodeNode aspectNode;

    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TMP_Text text;

    /// <summary>
    /// Initializes the AspectButtonUI with the given parameters.
    /// </summary>
    /// <param name="treeUI">The AspectsTreeUI instance.</param>
    /// <param name="aspectsManager">The AspectsManager instance.</param>
    /// <param name="aspectNode">The AspectNodeNode instance.</param>
    public void Init(GameManager gameManager, AspectsUIPanel treeUI, AspectsManager aspectsManager, AspectNodeNode aspectNode)
    {
        this.gameManager = gameManager;
        aspectsTreeUI = treeUI;
        this.aspectsManager = aspectsManager;
        this.aspectNode = aspectNode;

        UpdateDisplayContents();
    }

    private void Start()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TMP_Text>();

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

        //aspectsTreeUI.UpdateTree();
    }

    /// <summary>
    /// Changes the appearance and behavior of the button based on the given aspect node.
    /// </summary>
    /// <param name="treeUI">The AspectsTreeUI instance.</param>
    /// <param name="aspectsManager">The AspectsManager instance.</param>
    /// <param name="aspectNode">The AspectNodeNode instance.</param>
    public void UpdateDisplayContents()
    {
        // only the next possible nodes will be interactable and on the same level as the first applied multi-node level node
        button.interactable = aspectsManager.CurrentAspectTree.GetNextUnappliedNodes().Contains(aspectNode) && aspectsManager.AspectTokens > 0 && aspectsManager.CurrentAspectTree.CanMultiNodeLevelNodeBeChosen(aspectNode);
        // make the button green if its been applied already
        buttonImage.color = aspectNode.IsApplied ? Color.green : buttonImage.color;

        if (aspectNode.GetType() == typeof(ComboAspectNodeNode))
        {
            ComboAspectNodeNode comboAspectNode = aspectNode as ComboAspectNodeNode;
            text.text = comboAspectNode == null ? $"Combo missing" : $"{comboAspectNode.DisplayName}";
        }

        if (aspectNode.GetType() == typeof(StatusEffectAspectNodeNode))
        {
            StatusEffectAspectNodeNode statusEffectAspectNode = aspectNode as StatusEffectAspectNodeNode;

            text.text = statusEffectAspectNode.StatusEffect == null ? $"Status effect missing" : $"{statusEffectAspectNode.DisplayName}";
        }
    }
}
