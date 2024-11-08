using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    private string currentAugment = null;
    private int unlockedLevel = 0;
    private int lastLevelAdded = 0;

    [SerializeField] public GameObject Player;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool AddAugment<T>()
        where T : Augment
    {
        T augment = gameObject.GetComponent<T>();

        if (augment == null)
        {
            Debug.LogWarning("[AugmentManager] could not enable Augment: could not find object");
            return false;
        }

        if (currentAugment == null)
        {
            currentAugment = augment.AugmentName;
        }
        else if (augment.AugmentName != currentAugment)
        {
            Debug.LogWarning("[AugmentManager] could not enable Augment: augment mismatch");
            return false;
        }

        if (augment.Level > unlockedLevel || augment.Level != lastLevelAdded + 1)
        {
            Debug.LogWarning("[AugmentManager] could not enable Augment: level mismatch");
            return false;
        }

        augment.enabled = true;
        lastLevelAdded = augment.Level;
        return true;
    }

    public void IncrementLevel()
    {
        unlockedLevel++;
    }
}
