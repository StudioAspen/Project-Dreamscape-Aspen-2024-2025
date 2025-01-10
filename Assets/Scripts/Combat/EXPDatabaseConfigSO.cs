using System;
using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "EXP Database Config", menuName = "Configs/EXP Database")]
public class EXPDatabaseConfigSO : ScriptableObject
{
    [Serializable]
    public class EXPValueEntry
    {
        [field: SerializeField] public int Base { get; private set; }
        [field: SerializeField] public int Elite { get; private set; }
    }

    [field: Header("EXP Database")]
    [field: SerializeField, SerializedDictionary("Enemy Type", "EXP Values")]
    public SerializedDictionary<string, EXPValueEntry> EXPDatabase { get; private set; } = new SerializedDictionary<string, EXPValueEntry>();

    [Header("Debug Button")]
    [SerializeField, Tooltip("Button that checks the for any errors in the EXPDatabase dictionary.")]
    private bool checkForAnyErrors;

    private void OnValidate()
    {
        if (checkForAnyErrors)
        {
            checkForAnyErrors = false;
            CheckKeyTypes();
            CheckEXPValues();
        }
    }

    /// <summary>
    /// Retrieves the EXP value associated with the specified type.
    /// If the enemy type is elite, the elite EXP value is returned.
    /// If the type is not found, logs an error and returns 0.
    /// </summary>
    /// <param name="type">The type to retrieve the EXP value for.</param>
    /// <param name="isEnemyElite">Whether the enemy is elite.</param>
    /// <returns>The EXP value for the specified type.</returns>
    public int GetEXPFromType(Type type, bool isEnemyElite = false)
    {
        if (!EXPDatabase.ContainsKey(type.ToString()))
        {
            Debug.LogError($"No EXP value found for type {type}.");
            return 0;
        }

        return isEnemyElite ? EXPDatabase[type.ToString()].Elite : EXPDatabase[type.ToString()].Base;
    }

    /// <summary>
    /// Checks the key types in the EXPDatabase dictionary and logs an error if a type is not found or is not a subclass of Entity.
    /// </summary>
    private void CheckKeyTypes()
    {
        int invalidKeyCount = 0;

        foreach (var keyValuePair in EXPDatabase)
        {
            Type type = Type.GetType(keyValuePair.Key);
            if (type == null)
            {
                Debug.LogError($"Type {keyValuePair.Key} is not a valid in {name}.");
                invalidKeyCount++;
                continue;
            }
            if (!type.IsSubclassOf(typeof(Entity)))
            {
                Debug.LogError($"Type {keyValuePair.Key} is not a subclass of Entity type in {name}.");
                invalidKeyCount++;
                continue;
            }
        }

        if(invalidKeyCount == 0)
        {
            Debug.Log($"All keys in this dictionary are valid Entity subclasses in {name}.");
        }
    }

    /// <summary>
    /// Checks the EXP values in the EXPDatabase dictionary and logs warnings for any issues.
    /// </summary>
    private void CheckEXPValues()
    {
        int warningCount = 0;

        foreach (var keyValuePair in EXPDatabase)
        {
            EXPValueEntry expValueEntry = keyValuePair.Value;

            if (expValueEntry.Base < 0)
            {
                Debug.LogError($"Base EXP value for {keyValuePair.Key} is negative.");
                warningCount++;
            }
            if (expValueEntry.Elite < 0)
            {
                Debug.LogError($"Elite EXP value for {keyValuePair.Key} is negative.");
                warningCount++;
            }
            if (expValueEntry.Base == 0)
            {
                Debug.LogWarning($"Base EXP value for {keyValuePair.Key} is equal to 0.");
                warningCount++;
            }
            if (expValueEntry.Elite == 0)
            {
                Debug.LogWarning($"Elite EXP value for {keyValuePair.Key} is equal to 0.");
                warningCount++;
            }
            if (expValueEntry.Elite <= expValueEntry.Base)
            {
                Debug.LogWarning($"Elite EXP value for {keyValuePair.Key} is less than or equal to the base EXP value.");
                warningCount++;
            }
        }

        if (warningCount == 0)
        {
            Debug.Log($"There are no issues with the EXP Values in {name}.");
        }
    }
}