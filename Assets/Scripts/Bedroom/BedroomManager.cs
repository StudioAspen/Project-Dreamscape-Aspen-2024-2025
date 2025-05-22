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
    }

    private void Start()
    {
        SelectFirstItem();
    }

    public void SelectNextItem()
    {
        currentItemIndex++;
        currentItemIndex %= bedroomItems.Count;
        SelectedItem = bedroomItems[currentItemIndex];

        TweenCameraToTransform(SelectedItem.CameraTargetTransform);
    }

    public void SelectPreviousItem()
    {
        currentItemIndex--;
        if (currentItemIndex < 0)
        {
            currentItemIndex = bedroomItems.Count - 1;
        }
        SelectedItem = bedroomItems[currentItemIndex];

        TweenCameraToTransform(SelectedItem.CameraTargetTransform);
    }

    public void SelectFirstItem()
    {
        currentItemIndex = 0;
        SelectedItem = bedroomItems[currentItemIndex];

        TweenCameraToTransform(SelectedItem.CameraTargetTransform);
    }

    private void TweenCameraToTransform(Transform targetTransform)
    {
        DOTween.Kill(bedroomCameraTransform);

        bedroomCameraTransform.DOMove(targetTransform.position, cameraTweenDuration).SetEase(cameraTweenEase);

        // get quaternion lookat from bedroomcamera to the target transform
        Vector3 lookAtDirection = targetTransform.position - bedroomCameraTransform.position;
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
