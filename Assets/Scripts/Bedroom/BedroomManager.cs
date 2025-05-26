using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BedroomManager : MonoBehaviour
{
    [SerializeField] private List<BedroomItem> bedroomItems;

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

    // Debug button
    [Header("Debug")]
    public bool ClearSaveData = false;
    private void OnValidate()
    {
        if(ClearSaveData)
        {
            SaveLoadManager.ClearBedroomSaveData();
            ClearSaveData = false;
            Debug.LogWarning("Cleared bedroom save data.");
        }
    }

    private void Awake()
    {
        originalCameraTransform = new GameObject("OriginalCameraTransform").transform;
        originalCameraTransform.position = bedroomCameraTransform.position;
        originalCameraTransform.rotation = bedroomCameraTransform.rotation;

        Currency = 100000;

        TryLoadSavedItems();

        SelectFirstItem();
    }

    private void TryLoadSavedItems()
    {
        HashSet<int> activatedItemIDs = SaveLoadManager.LoadActivatedBedroomItemIDs();
        if (activatedItemIDs == null)
        {
            Debug.LogWarning("Loaded items list is null. All items will be deactive.");
            return;
        }

        Debug.Log($"Loading items from save file");
        for(int i = 0; i < bedroomItems.Count; i++)
        {
            if (bedroomItems[i] == null)
            {
                Debug.LogWarning($"Bedroom item at index {i} is null.");
                continue;
            }

            if (bedroomItems[i].Config == null)
            {
                Debug.LogWarning($"Bedroom item config at index {i} is null.");
                continue;
            }

            if (!activatedItemIDs.Contains(bedroomItems[i].Config.UniqueID)) continue;

            ActivateItem(bedroomItems[i]);
        }
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

        if(item.Config == null)
        {
            Debug.LogWarning($"Item config is null, can't purchase.");
            return false;
        }

        if (!SpendCurrency(item.Config.Cost))
        {
            return false;
        }

        if (!ActivateItem(item))
        {
            return false;
        }

        Debug.Log($"Purchased item: {item.Config.DisplayName}");
        SaveLoadManager.SaveActivatedBedroomItems(bedroomItems);
        return true;
    }

    public void TryPurchaseSelectedItem()
    {
        PurchaseItem(SelectedItem);
    }
    #endregion
}
