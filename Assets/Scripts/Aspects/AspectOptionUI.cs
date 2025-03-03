using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AspectOptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private AspectsUIPanel aspectsUIPanel;
    private AspectTree aspectTree;
    private AspectsManager aspectsManager;
    private GameManager gameManager;
    private Button optionsButton;
    private int optionsIndex;

    [Header("References")]
    [SerializeField] private Image diamondImage;
    [SerializeField] private RectTransform diamondEndTransform;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private GameObject singleContentObject;
    [SerializeField] private TMP_Text singleUpgradeText;
    [SerializeField] private TMP_Text singleDescriptionText;
    [SerializeField] private GameObject doubleContentObject;
    [SerializeField] private TMP_Text leftUpgradeText;
    [SerializeField] private TMP_Text leftDescriptionText;    
    [SerializeField] private TMP_Text rightUpgradeText;
    [SerializeField] private TMP_Text rightDescriptionText;

    [Header("Config")]
    [SerializeField] private float diamondImageRiseDuration = 0.1f;
    [SerializeField] private Ease diamondImageRiseEase = Ease.OutCubic;
    [SerializeField] private float contentsFadeInDelay = 0.075f;
    [SerializeField] private float contentsFadeInDuration = 0.1f;

    private List<AspectNodeNode> aspectNodes = new();

    private Color textStartColor;
    private Vector3 diamondImageStartPosition;

    private bool isSingle;
    private bool isSelected;

    public void Init(AspectsUIPanel aspectsUIPanel, AspectTree aspectTree, AspectsManager aspectsManager, GameManager gameManager, int index)
    {
        this.aspectsUIPanel = aspectsUIPanel;
        this.aspectTree = aspectTree;
        this.aspectsManager = aspectsManager;
        this.gameManager = gameManager;
        optionsIndex = index;
        aspectNodes = new();

        titleText.text = $"{aspectTree.DisplayName}";
        List<AspectNodeNode> nextNodes = aspectTree.GetNextUnappliedNodes();
        isSingle = nextNodes.Count != 2;
        if (isSingle)
        {
            singleUpgradeText.text = $"{nextNodes[0].DisplayName}";
            singleDescriptionText.text = $"{nextNodes[0].Description}";
            aspectNodes.Add(nextNodes[0]);
        }
        else
        {
            leftUpgradeText.text = $"{nextNodes[0].DisplayName}";
            leftDescriptionText.text = $"{nextNodes[0].Description}";
            rightUpgradeText.text = $"{nextNodes[1].DisplayName}";
            rightDescriptionText.text = $"{nextNodes[1].Description}";
            aspectNodes.Add(nextNodes[0]);
            aspectNodes.Add(nextNodes[1]);
        }

        ResetToDefault();
    }

    public void AssignOptionsIndex(int index)
    {
        optionsIndex = index;
    }

    private void Awake()
    {
        optionsButton = GetComponent<Button>();

        textStartColor = titleText.color;
        diamondImageStartPosition = diamondImage.transform.localPosition;

        optionsButton.onClick.AddListener(OptionsButton_OnClick);
    }

    private void OnDestroy()
    {
        optionsButton.onClick.RemoveListener(OptionsButton_OnClick);
    }

    private void OptionsButton_OnClick()
    {
        if (isSingle)
        {
            aspectNodes[0].ApplyAspect(aspectsManager);
            aspectsManager.ConsumeAspectToken();
            if(aspectsManager.AspectTokens > 0)
            {
                Init(aspectsUIPanel, aspectTree, aspectsManager, gameManager, optionsIndex);
            }
            else
            {
                gameManager.ChangeState(GameState.BIOME_SELECTION);
            }
        }
        else
        {
            if (aspectsUIPanel.IsSelectingBranch)
            {
                aspectsUIPanel.DeselectBranchOptionUI();
                return;
            }
            aspectsUIPanel.SelectBranchOptionUI(optionsIndex);
        }
    }

    #region Hovering/Selecting
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselect(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (aspectsUIPanel.IsSelectingBranch) return;

        isSelected = true;

        PlaySelectedAnimation();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (aspectsUIPanel.IsSelectingBranch) return;

        isSelected = false;

        PlayDeselectedAnimation();
    }

    public void OnSingleContentSelect(BaseEventData eventData)
    {
        if (aspectsUIPanel.IsSelectingBranch) return;
        Debug.Log("Single selected");
    }

    public void OnSingleContentDeselect(BaseEventData eventData)
    {
        if (aspectsUIPanel.IsSelectingBranch) return;
        Debug.Log("Single deselected");
    }

    public void OnLeftContentSelect(BaseEventData eventData)
    {
        if (!aspectsUIPanel.IsSelectingBranch) return;
        Debug.Log("Left selected");
    }

    public void OnLeftContentDeselect(BaseEventData eventData)
    {
        if (!aspectsUIPanel.IsSelectingBranch) return;
        Debug.Log("Left deselected");
    }

    public void OnRightContentSelect(BaseEventData eventData)
    {
        if (!aspectsUIPanel.IsSelectingBranch) return;
        Debug.Log("Right selected");
    }

    public void OnRightContentDeselect(BaseEventData eventData)
    {
        if (!aspectsUIPanel.IsSelectingBranch) return;
        Debug.Log("Right deselected");
    }
    #endregion

    private void KillTweens()
    {
        DOTween.Kill($"{this}ContentIn");
        DOTween.Kill($"{this}ContentInDelay");
        DOTween.Kill($"{this}DiamondOut");
        DOTween.Kill($"{this}DiamondOutDelay");
        diamondImage.DOKill();
        titleText.DOKill();
        singleUpgradeText.DOKill();
        singleDescriptionText.DOKill();
        leftUpgradeText.DOKill();
        leftDescriptionText.DOKill();
        rightUpgradeText.DOKill();
        rightDescriptionText.DOKill();
    }

    private void ResetToDefault()
    {
        KillTweens();

        isSelected = false;

        diamondImage.transform.localPosition = diamondImageStartPosition;

        titleText.color = Color.clear;
        singleUpgradeText.color = Color.clear;
        singleDescriptionText.color = Color.clear;
        leftUpgradeText.color = Color.clear;
        leftDescriptionText.color = Color.clear;
        rightUpgradeText.color = Color.clear;
        rightDescriptionText.color = Color.clear;

        titleText.gameObject.SetActive(false);
        singleContentObject.gameObject.SetActive(false);
        doubleContentObject.gameObject.SetActive(false);

        optionsButton.interactable = false;
    }

    private void PlaySelectedAnimation()
    {
        KillTweens();

        titleText.gameObject.SetActive(true);
        singleContentObject.SetActive(isSingle);
        doubleContentObject.SetActive(!isSingle);

        // Move diamond up
        diamondImage.transform.DOLocalMove(diamondEndTransform.localPosition, diamondImageRiseDuration).SetEase(diamondImageRiseEase).SetUpdate(true);

        Sequence contentsFadeInSequence = DOTween.Sequence().SetUpdate(true);
        contentsFadeInSequence.Append(titleText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true));
        if (isSingle)
        {
            contentsFadeInSequence.Join(singleUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(singleDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .SetId($"{this}ContentIn")
                .Pause(); // So it doesn't immediately play
        }
        else
        {
            contentsFadeInSequence.Join(leftDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(leftUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(rightDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(rightUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .SetId($"{this}ContentIn")
                .Pause(); // So it doesn't immediately play
        }

        // Fade in the text after a delay
        DOVirtual.DelayedCall(contentsFadeInDelay, () => contentsFadeInSequence.Play(), true)
            .SetId($"{this}ContentInDelay")
            .OnComplete(() => { optionsButton.interactable = true; });
    }

    private void PlayDeselectedAnimation()
    {
        KillTweens();

        // Fade out text
        titleText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
        if (isSingle)
        {
            singleUpgradeText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
            singleDescriptionText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
        }
        else
        {
            leftDescriptionText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
            leftUpgradeText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
            rightDescriptionText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
            rightUpgradeText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
        }

        // Move diamond down after a delay
        DOVirtual.DelayedCall(contentsFadeInDelay, () =>
        {
            diamondImage.transform.DOLocalMove(diamondImageStartPosition, diamondImageRiseDuration)
            .SetUpdate(true)
            .SetEase(diamondImageRiseEase)
            .OnComplete(() => ResetToDefault())
            .SetId($"{this}DiamondOut");
        }, true).SetId($"{this}DiamondOutDelay");
    }
}
