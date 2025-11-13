using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BedroomManager : MonoBehaviour
{
    private List<BedroomItem> bedroomItems;

    private int currentItemIndex = 0;
    public BedroomItem SelectedItem { get; private set; }

    #region Shop
    [field: SerializeField] public int Currency { get; private set; }
    #endregion

    [Header("Camera Config")]
    [SerializeField] private Transform bedroomCameraTransform;
    [SerializeField] private float cameraTweenDuration = 1f;
    [SerializeField] private Ease cameraTweenEase = Ease.InQuart;
    private Transform originalCameraTransform;

    // Debug button
    [Header("Debug")]
    public bool ClearBedroomSaveData = false;
    public bool ClearGameSaveData = false;
    private void OnValidate()
    {
        if(ClearBedroomSaveData)
        {
            SaveLoadManager.ClearBedroomSaveData();
            ClearBedroomSaveData = false;
            Debug.LogWarning("Cleared bedroom save data.");
        }

        if (ClearGameSaveData)
        {
            SaveLoadManager.ClearGameData();
            ClearGameSaveData = false;
            Debug.LogWarning("Cleared game save data.");
        }
    }

    private void Awake()
    {
        // Automatically grabs all bedroom items in the scene. No need to drag them in now.
        bedroomItems = GetSceneBedroomItems();

        originalCameraTransform = CreateOriginalCameraTransform();

        LoadSave();

        SelectFirstItem();
    }

    /// <summary>
    /// Gets the list of bedroom items in the scene. Ordered by their UniqueID in the config.
    /// </summary>
    private List<BedroomItem> GetSceneBedroomItems()
    {
        List<BedroomItem> items = new List<BedroomItem>(FindObjectsOfType<BedroomItem>());
        return items.OrderBy(item => item.Config?.UniqueID ?? int.MaxValue).ToList();
    }

    private Transform CreateOriginalCameraTransform()
    {
        Transform newTransform = new GameObject("OriginalCameraTransform").transform;
        newTransform.position = bedroomCameraTransform.position;
        newTransform.rotation = bedroomCameraTransform.rotation;
        return newTransform;
    }

    private void LoadSave()
    {
        BedroomSaveData bedroomSaveData = SaveLoadManager.LoadBedroomData();

        Currency = bedroomSaveData.Currency;

        HashSet<int> activatedItemIDs = new HashSet<int>(bedroomSaveData.ActivatedItemIDs);
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

        bedroomCameraTransform.DORotateQuaternion(item.CameraTargetTransform.rotation, cameraTweenDuration).SetEase(cameraTweenEase);
        bedroomCameraTransform.DOMove(targetPosition, cameraTweenDuration).SetEase(cameraTweenEase);

        //old way
        //Vector3 lookAtDirection = item.transform.position - targetPosition;
        //Quaternion lookAtRotation = Quaternion.LookRotation(lookAtDirection, Vector3.up);
        //bedroomCameraTransform.DORotateQuaternion(lookAtRotation, cameraTweenDuration).SetEase(cameraTweenEase);
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
        SaveBedroomData();
        return true;
    }

    public void TryPurchaseSelectedItem()
    {
        PurchaseItem(SelectedItem);
    }
    #endregion

    private void SaveBedroomData()
    {
        List<int> activatedItemIDs = new();
        foreach (var item in bedroomItems)
        {
            if (item == null || item.Config == null)
            {
                Debug.LogWarning("Item or Item Config is null, skipping save.");
                continue;
            }
            if (!item.IsActivated) continue;
            activatedItemIDs.Add(item.Config.UniqueID);
        }

        BedroomSaveData bedroomSaveData = new BedroomSaveData { 
            Currency = Currency,
            ActivatedItemIDs = activatedItemIDs,
        };

        SaveLoadManager.SaveBedroomData(bedroomSaveData);
    }
}
