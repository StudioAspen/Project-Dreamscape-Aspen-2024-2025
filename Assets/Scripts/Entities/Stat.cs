using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [field: SerializeField] public float BaseValue { get; private set; }
    [SerializeField] private List<float> multipliers = new List<float>();
    [field: SerializeField] public float FlatIncrease { get; private set; }

    public Stat(float baseValue)
    {
        BaseValue = baseValue;
    }

    public void SetBaseValue(float newBaseValue)
    {
        BaseValue = newBaseValue;
    }

    public void AddMultiplier(float multiplier)
    {
        multipliers.Add(multiplier);
    }

    public void RemoveMultiplier(float multiplier)
    {
        multipliers.Remove(multiplier);
    }

    public void AddFlatAmount(float amount)
    {
        FlatIncrease += amount;
    }

    public float GetFloatValue()
    {
        float finalValue = BaseValue;
        foreach(float multiplier in multipliers)
        {
            finalValue *= multiplier;
        }
        finalValue += FlatIncrease;
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
        foreach (float multiplier in multipliers)
        {
            finalMultiplier *= multiplier;
        }
        return finalMultiplier;
    }

    public void ClearMultipliers()
    {
        multipliers.Clear();
    }
}
