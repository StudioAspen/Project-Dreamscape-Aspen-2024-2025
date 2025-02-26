using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat : ISerializationCallbackReceiver
{
    public class Buffs
    {
        public List<float> Multipliers = new();
        public float FlatIncrease;
    }

    [field: SerializeField] public float BaseValue { get; private set; }
    private Dictionary<object, Buffs> buffsDictionary = new();

    public Stat(float baseValue)
    {
        BaseValue = baseValue;
        buffsDictionary = new();
    }

    public void SetBaseValue(float newBaseValue)
    {
        BaseValue = newBaseValue;
    }

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        // Ensures dictionary is initialized after deserialization
        buffsDictionary ??= new Dictionary<object, Buffs>();
    }

    /// <summary>
    /// Adds a multiplier from an instance of a script
    /// </summary>
    /// <param name="multiplier">The multiplier to apply</param>
    /// <param name="source">The instance of the script that applies the multiplier</param>
    public void AddMultiplier(float multiplier, object source)
    {
        if (buffsDictionary.ContainsKey(source))
        {
            buffsDictionary[source].Multipliers.Add(multiplier);
            return;
        }

        Buffs newBuffs = new Buffs {
            Multipliers = new List<float>() { multiplier },
            FlatIncrease = 0
        };
        buffsDictionary.Add(source, newBuffs);
    }

    /// <summary>
    /// Removes a multiplier from an instance of a script
    /// </summary>
    /// <param name="multiplier">The multiplier to apply</param>
    /// <param name="source">The instance of the script that applied the multiplier</param>
    public void RemoveMultiplier(float multiplier, object source)
    {
        if (!buffsDictionary.ContainsKey(source)) return;

        buffsDictionary[source].Multipliers.Remove(multiplier);
    }

    /// <summary>
    /// Adds a flat amount from an instance of a script
    /// </summary>
    /// <param name="amount">The flat amount to apply</param>
    /// <param name="source">The instance of the script that applied the flat amount</param>
    public void AddFlatAmount(float amount, object source)
    {
        if (buffsDictionary.ContainsKey(source))
        {
            buffsDictionary[source].FlatIncrease += amount;
            return;
        }

        Buffs newBuffs = new Buffs
        {
            Multipliers = new List<float>(),
            FlatIncrease = amount
        };
        buffsDictionary.Add(source, newBuffs);
    }

    /// <summary>
    /// Removes all the multipliers applied by a script
    /// </summary>
    /// <param name="source">The instance of the script that applied the multiplers</param>
    public void ClearMultipliersFromSource(object source)
    {
        if (!buffsDictionary.ContainsKey(source)) return;
        buffsDictionary[source].Multipliers.Clear();
    }

    /// <summary>
    /// Removes all the flat increases applied by a script
    /// </summary>
    /// <param name="source">The instance of the script that applied the flat increases</param>
    public void ClearFlatIncreaseFromSource(object source)
    {
        if (!buffsDictionary.ContainsKey(source)) return;
        buffsDictionary[source].FlatIncrease = 0;
    }

    /// <summary>
    /// Removes all the buffs applied by a script
    /// </summary>
    /// <param name="source">The instance of the script that applied the buffs</param>
    public void ClearBuffsFromSource(object source)
    {
        if (!buffsDictionary.ContainsKey(source)) return;
        buffsDictionary.Remove(source);
    }

    /// <summary>
    /// Removes all the buffs on this stat
    /// </summary>
    public void ClearAllBuffs()
    {
        buffsDictionary.Clear();
    }

    public float GetFloatValue()
    {
        float finalValue = BaseValue;
        if (buffsDictionary.Count == 0) return finalValue;
        // Apply all the multipliers first
        foreach(Buffs buffs in buffsDictionary.Values)
        {
            foreach(float multiplier in buffs.Multipliers)
            {
                finalValue *= multiplier;
            }
        }
        // Then apply all the flat increases
        foreach (Buffs buffs in buffsDictionary.Values)
        {
            finalValue += buffs.FlatIncrease;
        }
        return finalValue;
    }

    /// <summary>
    /// Rounds the final value of the stat to the nearest int.
    /// </summary>
    /// <returns>The rounded int value of the stat.</returns>
    public int GetIntValue()
    {
        return Mathf.RoundToInt(GetFloatValue());
    }

    /// <summary>
    /// Gets the final multiplier amount.
    /// </summary>
    /// <returns>The final multiplier amount.</returns>
    public float GetTotalMultiplier()
    {
        float finalMultiplier = 1f;
        foreach (Buffs buffs in buffsDictionary.Values)
        {
            foreach (float multiplier in buffs.Multipliers)
            {
                finalMultiplier *= multiplier;
            }
        }
        return finalMultiplier;
    }

    /// <summary>
    /// Gets the final flat increase amount.
    /// </summary>
    /// <returns>The final flat increase amount.</returns>
    public float GetTotalFlatIncreass()
    {
        float finalIncrease = 0;
        foreach (Buffs buffs in buffsDictionary.Values)
        {
            finalIncrease += buffs.FlatIncrease;
        }
        return finalIncrease;
    }
}