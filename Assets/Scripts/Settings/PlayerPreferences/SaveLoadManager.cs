using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveLoadManager {
    private static string playerPreferencesPath = $"{Application.persistentDataPath}/playerPreferencesData.json";

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
    
}
