using System;
using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "EXP Database Config", menuName = "Configs/EXP Database")]
public class EXPDatabaseConfigSO : ScriptableObject
{
    [field: Header("EXP Database")]
    [field: SerializeField] public SerializedDictionary<string, int> EXPDatabase { get; private set; } = new SerializedDictionary<string, int>();

    public int GetEXPFromType(Type type)
    {
        if (EXPDatabase.ContainsKey(type.ToString()))
        {
            return EXPDatabase[type.ToString()];
        }
        else
        {
            Debug.LogWarning($"No EXP value found for type {type}.");
            return 0;
        }
    }
}