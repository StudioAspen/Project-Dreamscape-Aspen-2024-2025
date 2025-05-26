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
    
    public static void SaveBedroomData(BedroomSaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(bedroomItemsPath, json);
        Debug.Log($"Bedroom items data {json} saved to {bedroomItemsPath}");
    }

    public static BedroomSaveData LoadBedroomData()
    {
        if (File.Exists(bedroomItemsPath))
        {
            string json = File.ReadAllText(bedroomItemsPath);
            BedroomSaveData loadedData = JsonUtility.FromJson<BedroomSaveData>(json);
            Debug.Log($"Bedroom item data {json} loaded from {bedroomItemsPath}");
            return loadedData;
        }
        Debug.LogWarning("No bedroom items data found, returning empty save");
        return new BedroomSaveData
        {
            Currency = 0,
            ActivatedItemIDs = new List<int>(),
        };
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
