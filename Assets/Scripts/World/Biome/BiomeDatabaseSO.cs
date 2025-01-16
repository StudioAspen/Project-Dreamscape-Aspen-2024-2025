using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome Database", menuName = "Biome Database")]
public class BiomeDatabaseSO : ScriptableObject
{
    [Serializable]
    public class BiomeData
    {
        [field: SerializeField] public string BiomeName { get; private set; } = "Biome Name";
        [field: SerializeField, TextArea(3, 20)] public string Description { get; private set; } = "Description of biome.";
        [field: SerializeField] public Color BiomeColor { get; private set; } = Color.black;
    }

    [field: Header("Biomes")]
    [field: SerializeField, SerializedDictionary("Biome Type", "Biome Data")]
    public SerializedDictionary<Biome, BiomeData> BiomesDictionary { get; private set; } = new SerializedDictionary<Biome, BiomeData>();

    [Header("Debug Button")]
    [SerializeField, Tooltip("Button that checks the for any errors in the BiomeDatabase dictionary.")]
    private bool checkForAnyErrors;

    private void OnValidate()
    {
        if (checkForAnyErrors)
        {
            checkForAnyErrors = false;
            CheckKeys();
        }
    }

    /// <summary>
    /// Checks the keys in the BiomesDictionary to ensure that there are no missing keys.
    /// If a key is missing, it logs an error message.
    /// </summary>
    private void CheckKeys()
    {
        int errorCount = 0;

        foreach (Biome biome in Enum.GetValues(typeof(Biome)))
        {
            if (!BiomesDictionary.ContainsKey(biome))
            {
                Debug.LogError($"Missing key for Biome: {biome}");
                errorCount++;
            }
        }

        if(errorCount == 0)
        {
            Debug.Log("BiomeDatabase has no errors.");
        }
    }
}

public enum Biome
{
    DREAM,
    FIRE,
    FOOD,
    OTHER_BIOME
}