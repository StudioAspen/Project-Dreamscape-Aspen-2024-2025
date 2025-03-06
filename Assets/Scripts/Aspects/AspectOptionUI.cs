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
    private bool representsRuntimeInstance;
    private AspectsUIPanel aspectsUIPanel;
    private AspectTree aspectTree;
    private AspectsManager aspectsManager;
    private GameManager gameManager;
    public Button OptionsButton { get; private set; }
    private int optionsIndex;

    [Header("References")]
    [SerializeField] private Image diamondImage;
    [SerializeField] private RectTransform diamondEndTransform;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button singleContentButton;
    [SerializeField] private TMP_Text singleUpgradeText;
    [SerializeField] private TMP_Text singleDescriptionText;
    [SerializeField] private GameObject doubleContentObject;
    [SerializeField] private Button leftContentButton;
    [SerializeField] private TMP_Text leftUpgradeText;
    [SerializeField] private TMP_Text leftDescriptionText;    
    [SerializeField] private Button rightContentButton;
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
    private bool isCompleted;

    public void Init(bool isRuntimeInstance, AspectsUIPanel aspectsUIPanel, AspectTree aspectTree, AspectsManager aspectsManager, GameManager gameManager, int index)
    {
        representsRuntimeInstance = isRuntimeInstance;
        this.aspectsUIPanel = aspectsUIPanel;
        this.aspectTree = aspectTree;
        this.aspectsManager = aspectsManager;
        this.gameManager = gameManager;
        optionsIndex = index;
        aspectNodes = new();
        isCompleted = aspectTree.IsCompleted();

        diamondImage.sprite = aspectTree.AspectSprite;
        titleText.text = $"{aspectTree.DisplayName}";
        descriptionText.text = $"{(isCompleted ? "Completed\n" : "")}{aspectTree.Description}";

        textStartColor = aspectTree.AspectTextColor;

        if (isCompleted)
        {
            ResetToDefault();
            if (aspectsUIPanel.IsSelectingAspect) aspectsUIPanel.DeselectOptionUI();
            if (isSelected) OnSelect(null);
            return;
        }

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
        if(aspectsUIPanel.IsSelectingAspect) aspectsUIPanel.DeselectOptionUI();

        if (isSelected)
        {
            OnSelect(null);
        }
    }

    private void ConvertToRuntimeInstance(AspectTree runtimeInstace)
    {
        //Debug.Log($"Converted {gameObject.name}'s {runtimeInstace.DisplayName} to runtime instance");
        representsRuntimeInstance = true;
        aspectTree = runtimeInstace;

        List<AspectNodeNode> nextNodes = aspectTree.GetNextUnappliedNodes();
        aspectNodes = new() { nextNodes[0] };
    }

    public void AssignOptionsIndex(int index)
    {
        optionsIndex = index;
    }

    public void SetLockedNavigation()
    {
        Navigation newNavigation = OptionsButton.navigation;
        newNavigation.mode = Navigation.Mode.Explicit;

        if (isSingle)
        {
            newNavigation.selectOnUp = OptionsButton;
            newNavigation.selectOnDown = singleContentButton;
            newNavigation.selectOnLeft = OptionsButton;
            newNavigation.selectOnRight = OptionsButton;
        }
        else
        {
            newNavigation.selectOnUp = OptionsButton;
            newNavigation.selectOnDown = leftContentButton;
            newNavigation.selectOnLeft = OptionsButton;
            newNavigation.selectOnRight = OptionsButton;
        }

        OptionsButton.navigation = newNavigation;
    }

    private void Awake()
    {
        OptionsButton = GetComponent<Button>();

        diamondImageStartPosition = diamondImage.transform.localPosition;

        OptionsButton.onClick.AddListener(OptionsButton_OnClick);
    }

    private void OnDestroy()
    {
        OptionsButton.onClick.RemoveListener(OptionsButton_OnClick);
    }

    private void OptionsButton_OnClick()
    {
        if (isCompleted) return;

        if (aspectsUIPanel.IsSelectingAspect)
        {
            aspectsUIPanel.DeselectOptionUI();
            PlayNodesUnrevealAnimation();
        }
        else
        {
            aspectsUIPanel.SelectOptionUI(optionsIndex);
            PlayNodesRevealAnimation();
        }
    }

    public void OnSingleContentClick(BaseEventData eventData)
    {
        if (isCompleted) return;
        if (!aspectsUIPanel.IsSelectingAspect) return;

        if (!representsRuntimeInstance)
        {
            AspectTree runtimeAspectTree = aspectsManager.AddAspectTree(aspectTree);
            ConvertToRuntimeInstance(runtimeAspectTree);
        }
        
        aspectNodes[0].ApplyAspect(aspectsManager);
        aspectsManager.ConsumeAspectToken();
        if (aspectsManager.AspectTokens > 0)
        {
            Init(true, aspectsUIPanel, aspectTree, aspectsManager, gameManager, optionsIndex);
        }
        else
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
        }
    }

    public void OnLeftContentClick(BaseEventData eventData)
    {
        if (isCompleted) return;
        if (!aspectsUIPanel.IsSelectingAspect) return;

        if (!representsRuntimeInstance)
        {
            AspectTree runtimeAspectTree = aspectsManager.AddAspectTree(aspectTree);
            ConvertToRuntimeInstance(runtimeAspectTree);
        }

        aspectNodes[0].ApplyAspect(aspectsManager);
        aspectsManager.ConsumeAspectToken();
        if (aspectsManager.AspectTokens > 0)
        {
            Init(true, aspectsUIPanel, aspectTree, aspectsManager, gameManager, optionsIndex);
        }
        else
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
        }
    }

    public void OnRightContentClick(BaseEventData eventData)
    {
        if (isCompleted) return;
        if (!aspectsUIPanel.IsSelectingAspect) return;

        if (!representsRuntimeInstance)
        {
            AspectTree runtimeAspectTree = aspectsManager.AddAspectTree(aspectTree);
            ConvertToRuntimeInstance(runtimeAspectTree);
        }

        aspectNodes[1].ApplyAspect(aspectsManager);
        aspectsManager.ConsumeAspectToken();
        if (aspectsManager.AspectTokens > 0)
        {
            Init(true, aspectsUIPanel, aspectTree, aspectsManager, gameManager, optionsIndex);
        }
        else
        {
            gameManager.ChangeState(GameState.BIOME_SELECTION);
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
        if (aspectsUIPanel.IsSelectingAspect) return;

        isSelected = true;

        PlayOptionSelectedAnimation();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (aspectsUIPanel.IsSelectingAspect) return;

        isSelected = false;

        PlayOptionDeselectedAnimation();
    }

    public void OnSingleContentSelect(BaseEventData eventData)
    {
        if (isCompleted) return;
        if (!aspectsUIPanel.IsSelectingAspect) return;
        //Debug.Log("Single select");
        singleContentButton.transform.DOKill();
        singleContentButton.transform.DOScale(1.1f, 0.1f).SetUpdate(true);
    }

    public void OnSingleContentDeselect(BaseEventData eventData)
    {
        if (isCompleted) return;
        if (!aspectsUIPanel.IsSelectingAspect) return;
        //Debug.Log("Single deselect");
        singleContentButton.transform.DOKill();
        singleContentButton.transform.DOScale(1f, 0.1f).SetUpdate(true);
    }

    public void OnLeftContentSelect(BaseEventData eventData)
    {
        if (isCompleted) return;
        if (!aspectsUIPanel.IsSelectingAspect) return;
        //Debug.Log("Left select");
        leftContentButton.transform.DOKill();
        leftContentButton.transform.DOScale(1.1f, 0.1f).SetUpdate(true);
    }

    public void OnLeftContentDeselect(BaseEventData eventData)
    {
        if (isCompleted) return;
        if (!aspectsUIPanel.IsSelectingAspect) return;
        //Debug.Log("Left deselect");
        leftContentButton.transform.DOKill();
        leftContentButton.transform.DOScale(1f, 0.1f).SetUpdate(true);
    }

    public void OnRightContentSelect(BaseEventData eventData)
    {
        if (isCompleted) return;
        if (!aspectsUIPanel.IsSelectingAspect) return;
        //Debug.Log("Right select");
        rightContentButton.transform.DOKill();
        rightContentButton.transform.DOScale(1.1f, 0.1f).SetUpdate(true);
    }

    public void OnRightContentDeselect(BaseEventData eventData)
    {
        if (isCompleted) return;
        if (!aspectsUIPanel.IsSelectingAspect) return;
        //Debug.Log("Right deselect");
        rightContentButton.transform.DOKill();
        rightContentButton.transform.DOScale(1f, 0.1f).SetUpdate(true);
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
        descriptionText.DOKill();
        singleContentButton.transform.DOKill();
        singleUpgradeText.DOKill();
        singleDescriptionText.DOKill();
        doubleContentObject.transform.DOKill();
        leftContentButton.transform.DOKill();
        leftUpgradeText.DOKill();
        leftDescriptionText.DOKill();
        rightContentButton.transform.DOKill();
        rightUpgradeText.DOKill();
        rightDescriptionText.DOKill();
    }

    private void ResetToDefault()
    {
        KillTweens();

        isSelected = false;

        diamondImage.transform.localPosition = diamondImageStartPosition;

        titleText.color = Color.clear;
        descriptionText.color = Color.clear;
        singleUpgradeText.color = Color.clear;
        singleDescriptionText.color = Color.clear;
        leftUpgradeText.color = Color.clear;
        leftDescriptionText.color = Color.clear;
        rightUpgradeText.color = Color.clear;
        rightDescriptionText.color = Color.clear;

        singleContentButton.transform.localScale = Vector3.one;
        leftContentButton.transform.localScale = Vector3.one;
        rightContentButton.transform.localScale = Vector3.one;

        titleText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        singleContentButton.gameObject.SetActive(false);
        doubleContentObject.gameObject.SetActive(false);

        OptionsButton.interactable = false;
    }

    private void PlayNodesRevealAnimation()
    {
        KillTweens();

        // Fade out description text
        descriptionText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);

        if (isSingle)
        {
            singleContentButton.gameObject.SetActive(true);
            singleUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
            singleDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
        }
        else
        {
            doubleContentObject.SetActive(true);
            leftUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
            leftDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
            rightUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
            rightDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
        }
    }

    private void PlayNodesUnrevealAnimation()
    {
        KillTweens();

        // Fade in description
        descriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);

        if (isSingle)
        {
            singleUpgradeText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
            singleDescriptionText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true).OnComplete(() => singleContentButton.gameObject.SetActive(true));
        }
        else
        {
            leftUpgradeText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
            leftDescriptionText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
            rightUpgradeText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
            rightDescriptionText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true).OnComplete(() => doubleContentObject.SetActive(true));
        }
    }

    private void PlayOptionSelectedAnimation()
    {
        KillTweens();

        titleText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);

        // Move diamond up
        diamondImage.transform.DOLocalMove(diamondEndTransform.localPosition, diamondImageRiseDuration).SetEase(diamondImageRiseEase).SetUpdate(true);

        Sequence contentsFadeInSequence = DOTween.Sequence().SetUpdate(true);
        contentsFadeInSequence.Append(titleText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true));
        contentsFadeInSequence.Join(descriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true));
        contentsFadeInSequence.SetId($"{this}ContentIn");
        contentsFadeInSequence.Pause();

        // Fade in the text after a delay
        DOVirtual.DelayedCall(contentsFadeInDelay, () => contentsFadeInSequence.Play(), true)
            .SetId($"{this}ContentInDelay")
            .OnComplete(() => { OptionsButton.interactable = true; });
    }

    private void PlayOptionDeselectedAnimation()
    {
        KillTweens();

        // Fade out text
        titleText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);
        descriptionText.DOColor(Color.clear, contentsFadeInDuration).SetUpdate(true);

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
