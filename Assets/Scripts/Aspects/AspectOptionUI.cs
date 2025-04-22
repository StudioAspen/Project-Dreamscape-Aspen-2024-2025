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
    public Button OptionsButton { get; private set; }

    [Header("Diamond")]
    [SerializeField] private Image diamondImage;
    [SerializeField] private RectTransform diamondEndTransform;

    [Header("Aspect Title/Description")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Config")]
    [SerializeField] private float diamondImageFadeDuration = 0.1f;
    [SerializeField] private Ease diamondImageFadeEase = Ease.OutCubic;
    [SerializeField] private float contentsFadeInDelay = 0.075f;
    [SerializeField] private float contentsFadeInDuration = 0.1f;

    private Color textStartColor;
    private Vector3 diamondImageStartPosition;

    private bool isSelected;

    public void Init(bool isRuntimeInstance, AspectsUIPanel aspectsUIPanel, AspectTree aspectTree)
    {
        representsRuntimeInstance = isRuntimeInstance;
        this.aspectsUIPanel = aspectsUIPanel;
        this.aspectTree = aspectTree;

        diamondImage.sprite = aspectTree.AspectSprite;
        titleText.text = $"{aspectTree.DisplayName}";
        descriptionText.text = $"{aspectTree.Description}";

        textStartColor = aspectTree.AspectTextColor;

        ResetToDefault();

        if (isSelected)
        {
            OnSelect(null);
        }
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

    public void TryConvertToRuntimeInstance(AspectTree tree)
    {
        if (tree.DisplayName == aspectTree.DisplayName)
        {
            aspectTree = tree;
            representsRuntimeInstance = true;
        }
    }

    private void OptionsButton_OnClick()
    {
        aspectsUIPanel.OpenAspectTreeViewer(aspectTree, representsRuntimeInstance);
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
        isSelected = true;

        PlayOptionSelectedAnimation();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        PlayOptionDeselectedAnimation();
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
    }

    private void ResetToDefault()
    {
        KillTweens();

        isSelected = false;

        diamondImage.transform.localPosition = diamondImageStartPosition;

        titleText.color = Color.clear;
        descriptionText.color = Color.clear;

        titleText.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);

        OptionsButton.interactable = false;
    }

    private void PlayOptionSelectedAnimation()
    {
        KillTweens();

        titleText.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);

        // Move diamond up
        diamondImage.transform.DOLocalMove(diamondEndTransform.localPosition, diamondImageFadeDuration).SetEase(diamondImageFadeEase).SetUpdate(true);

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
            diamondImage.transform.DOLocalMove(diamondImageStartPosition, diamondImageFadeDuration)
            .SetUpdate(true)
            .SetEase(diamondImageFadeEase)
            .OnComplete(() => ResetToDefault())
            .SetId($"{this}DiamondOut");
        }, true).SetId($"{this}DiamondOutDelay");
    }
}
