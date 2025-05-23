using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveLoadManager {
    private static string playerPreferencesPath = $"{Application.persistentDataPath}/playerPreferencesData.json";
    private static string bedroomItemsPath = $"{Application.persistentDataPath}/bedroomItemsData.json";

    public static void SavePlayerPreferences(PlayerPreferencesData playerPreferences) {
        string json = JsonUtility.ToJson(playerPreferences, true);
        File.WriteAllText(playerPreferencesPath, json);
        Debug.Log($"Player preferences data saved to {playerPreferencesPath}");
    }

    public static PlayerPreferencesData LoadPlayerPreferences() {
        if (File.Exists(playerPreferencesPath)) {
            string json = File.ReadAllText(playerPreferencesPath);
            PlayerPreferencesData loadedPreferences = JsonUtility.FromJson<PlayerPreferencesData>(json);
            Debug.Log($"Player preferences data loaded from {playerPreferencesPath}");
            return loadedPreferences;
        }
        Debug.LogWarning("No player preferences data found");
        return null;
    }
    
    public static void SaveBedroomItems(List<BedroomItem> bedroomItems) {
        Dictionary<int, bool> activatedItemsDict = new();
        foreach (var item in bedroomItems) {
            if (item == null || item.Config == null) {
                Debug.LogWarning("Item or Item Config is null, skipping save.");
                continue;
            }
            // TODO:
            activatedItemsDict.Add(item.Config.UniqueID, item.IsActivated);
        }

        string json = JsonUtility.ToJson(activatedItemsDict, true);
        File.WriteAllText(bedroomItemsPath, json);
        Debug.Log($"Bedroom items data saved to {bedroomItemsPath}");
    }

    public static Dictionary<int, bool> LoadBedroomItems()
    {
        if (File.Exists(bedroomItemsPath))
        {
            string json = File.ReadAllText(bedroomItemsPath);
            Dictionary<int, bool> loadedItems = JsonUtility.FromJson<Dictionary<int, bool>>(json);
            Debug.Log($"Bedroom item data loaded from {playerPreferencesPath}");
            return loadedItems;
        }
        Debug.LogWarning("No player preferences data found");
        return null;
    }

    public static void ClearBedroomSaveData()
    {
        if (File.Exists(bedroomItemsPath))
        {
            File.Delete(bedroomItemsPath);
            Debug.Log($"Bedroom items data cleared from {bedroomItemsPath}");
        }
        else
        {
            Debug.LogWarning("No bedroom items data found to clear");
        }
    }
}
