using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BedroomManager : MonoBehaviour
{
    [SerializeField] private List<BedroomItem> bedroomItems = new();

    private int currentItemIndex = 0;
    public BedroomItem SelectedItem { get; private set; }

    #region Shop
    public int Currency { get; private set; }
    #endregion

    [Header("Camera Config")]
    [SerializeField] private Transform bedroomCameraTransform;
    [SerializeField] private float cameraTweenDuration = 1f;
    [SerializeField] private Ease cameraTweenEase = Ease.InQuart;
    private Transform originalCameraTransform;

    private void Awake()
    {
        originalCameraTransform = new GameObject("OriginalCameraTransform").transform;
        originalCameraTransform.position = bedroomCameraTransform.position;
        originalCameraTransform.rotation = bedroomCameraTransform.rotation;

        SelectFirstItem();
    }

    public void SelectNextItem()
    {
        currentItemIndex++;
        currentItemIndex %= bedroomItems.Count;
        SelectedItem = bedroomItems[currentItemIndex];

        TweenCameraToItem(SelectedItem);
    }

    public void SelectPreviousItem()
    {
        currentItemIndex--;
        if (currentItemIndex < 0)
        {
            currentItemIndex = bedroomItems.Count - 1;
        }
        SelectedItem = bedroomItems[currentItemIndex];

        TweenCameraToItem(SelectedItem);
    }

    public void SelectFirstItem()
    {
        currentItemIndex = 0;
        SelectedItem = bedroomItems[currentItemIndex];

        TweenCameraToItem(SelectedItem);
    }

    private void TweenCameraToItem(BedroomItem item)
    {
        DOTween.Kill(bedroomCameraTransform);

        Vector3 targetPosition = item.CameraTargetTransform.position;

        bedroomCameraTransform.DOMove(targetPosition, cameraTweenDuration).SetEase(cameraTweenEase);

        Vector3 lookAtDirection = item.transform.position - targetPosition;
        Quaternion lookAtRotation = Quaternion.LookRotation(lookAtDirection, Vector3.up);
        bedroomCameraTransform.DORotateQuaternion(lookAtRotation, cameraTweenDuration).SetEase(cameraTweenEase);
    }

    private bool ActivateItem(BedroomItem item)
    {
        if (item == null)
        {
            Debug.LogWarning($"Item is null, can't activate.");
            return false;
        }
        
        return item.Activate();
    }

    #region Shop Functions
    private bool SpendCurrency(int spendAmount)
    {
        if(spendAmount < 0)
        {
            Debug.LogWarning($"Spend amount is negative: {spendAmount}");
            return false;
        }

        if (spendAmount > Currency)
        {
            Debug.LogWarning($"Not enough currency to spend: {spendAmount} > {Currency}");
            return false;
        }

        Currency -= spendAmount;
        return true;
    }

    private bool PurchaseItem(BedroomItem item)
    {
        if(item == null)
        {
            Debug.LogWarning($"Item is null, can't purchase.");
            return false;
        }

        if (!ActivateItem(item))
        {
            return false;
        }

        if (!SpendCurrency(item.Config.Cost))
        {
            return false;
        }

        return true;
    }

    public void TryPurchaseSelectedItem()
    {
        PurchaseItem(SelectedItem);
    }
    #endregion
}
