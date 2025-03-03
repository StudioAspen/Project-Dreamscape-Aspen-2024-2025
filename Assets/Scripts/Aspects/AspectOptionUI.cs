using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class AspectOptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private AspectTree aspectTree;

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

    private Color textStartColor;
    private Vector3 diamondImageStartPosition;

    private bool isSingle;
    private bool isSelected;

    public void Init(AspectTree aspectTree)
    {
        this.aspectTree = aspectTree;

        titleText.text = $"{aspectTree.DisplayName}";
        List<AspectNodeNode> nextNodes = aspectTree.GetNextUnappliedNodes();
        isSingle = nextNodes.Count != 2;
        if (isSingle)
        {
            singleUpgradeText.text = $"{nextNodes[0].DisplayName}";
            singleDescriptionText.text = $"{nextNodes[0].Description}";
        }
        else
        {
            leftUpgradeText.text = $"{nextNodes[0].DisplayName}";
            leftDescriptionText.text = $"{nextNodes[0].Description}";
            rightUpgradeText.text = $"{nextNodes[1].DisplayName}";
            rightDescriptionText.text = $"{nextNodes[1].Description}";
        }
    }

    private void Awake()
    {
        textStartColor = titleText.color;
        diamondImageStartPosition = diamondImage.transform.localPosition;
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

        PlaySelectedAnimation();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        PlayDeselectedAnimation();
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
        if (isSingle)
        {
            contentsFadeInSequence.Append(titleText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(singleUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(singleDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .SetId($"{this}ContentIn")
                .Pause(); // So it doesn't immediately play
        }
        else
        {
            contentsFadeInSequence.Append(titleText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(leftDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(leftUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(rightDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .Join(rightUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true))
                .SetId($"{this}ContentIn")
                .Pause(); // So it doesn't immediately play
        }

        // Fade in the text after a delay
        DOVirtual.DelayedCall(contentsFadeInDelay, () => contentsFadeInSequence.Play(), true).SetId($"{this}ContentInDelay");
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
            leftDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
            leftUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
            rightDescriptionText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
            rightUpgradeText.DOColor(textStartColor, contentsFadeInDuration).SetUpdate(true);
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
