using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Dreamscape;

[CreateAssetMenu(fileName = "Data", menuName = "Combo/Default", order = 1)]
public class ComboDataSO : ScriptableObject
{

    [field: Header("Display")]
    [field: SerializeField] public string DisplayName { get; private set; } = "Combo";
    [field: SerializeField, TextArea(5, 20)] public string Description { get; private set; } = "Combo description";

    [field: Header("Combo Data")]
    [field: SerializeField] public List<ComboAction> ComboInputs { get; private set; } = new List<ComboAction>();
    [field: SerializeField] public AnimationClip ComboClip { get; private set; }
    [field: SerializeField] [field: Range(0.25f, 5f)] public float ComboClipAnimationSpeed { get; private set; } = 1f;

    [field: Header("Position Options")]
    [field: SerializeField, Tooltip("Root motion is when the animation moves the player")] public bool HasRootMotion { get; private set; } = true;
    [field: SerializeField, Tooltip("If this combo is meant to be performed midair")] public bool IsAirCombo { get; private set; }

    [field: Header("Damage")]
    [field: SerializeField] public float DamageMultiplier { get; private set; } = 1f;

    [field: Header("Launch Options")]
    [field: SerializeField, Tooltip("Determines if the hit will launch grounded enemies upwards")] public bool WillLaunchUpwards { get; private set; }
    [field: SerializeField, Tooltip("Upwards launch force on hit. Only works on airborne targets")] public float AirLaunchForce { get; private set; }
    [field: SerializeField, Tooltip("If this air combo is the end animation of the combo sequence")] public bool IsFinalAirCombo { get; private set; }

    [field: Header("Stun Options")]
    [field: SerializeField] public bool WillStun { get; private set; }
    [field: SerializeField] public float StunDuration { get; private set; }

    [field: Header("Impact Frames")]
    [field: SerializeField] public float ImpactFramesTimeScale { get; private set; } = 0.05f;
    [field: SerializeField] public float ImpactFramesDuration { get; private set; } = 0.25f;

    [field: Header("Weapon Size")]
    [field: SerializeField] public float WeaponScale { get; private set; } = 1f;
    [field: SerializeField] public float WeaponScalingDuration { get; private set; } = 0.1f;

    /// <summary>
    /// Checks to see if the given combo (starting from the front) is potentially in the other combo
    /// </summary>
    /// <param name="givenComboList"></param>
    /// <param name="otherComboList"></param>
    /// <returns></returns>
    public static bool IsPotentiallyIn(List<ComboAction> givenComboList, List<ComboAction> otherComboList)
    {
        if (otherComboList.Count > givenComboList.Count) return false;

        List<ComboAction> subList = givenComboList.GetRange(0, Mathf.Min(givenComboList.Count, otherComboList.Count));

        return IsIn(subList, otherComboList);
    }

    /// <summary>
    /// Checks to see if a given combo is in the other combo
    /// </summary>
    /// <param name="givenComboList"></param>
    /// <param name="otherComboList"></param>
    /// <returns></returns>
    public static bool IsIn(List<ComboAction> givenComboList, List<ComboAction> otherComboList)
    {
        if (givenComboList.Count > otherComboList.Count) return false;

        for (int i = 0; i < otherComboList.Count; i++)
        {
            int matches = 0;

            for (int j = 0; j < givenComboList.Count; j++)
            {
                if (i + j >= otherComboList.Count) break;

                ComboAction otherAction = otherComboList[i + j];
                ComboAction currAction = givenComboList[j];

                if (otherAction == currAction) matches++;
            }

            if (matches == givenComboList.Count) return true;
        }

        return false;
    }

    /// <summary>
    /// Returns the single action combo from a list of combos. If it doesn't exist returns null.
    /// </summary>
    /// <param name="combos"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static ComboDataSO GetSingleActionCombo(List<ComboDataSO> combos, ComboAction action)
    {
        foreach (ComboDataSO combo in combos)
        {
            if (combo.ComboInputs.Count == 1 && combo.ComboInputs.Contains(action)) return combo;
        }

        return null;
    }

    /// <summary>
    /// Returns the combo with the most number of actions given a list of combos
    /// </summary>
    /// <param name="combos"></param>
    /// <returns></returns>
    public static ComboDataSO GetLongestCombo(List<ComboDataSO> combos)
    {
        if (combos.Count == 0) return null;
        if (combos.Count == 1) return combos[0];

        ComboDataSO result = null;

        foreach (ComboDataSO combo in combos)
        {
            if (result == null)
            {
                result = combo;
                continue;
            }

            if (combo.ComboInputs.Count > result.ComboInputs.Count)
            {
                result = combo;
            }
        }

        return result;
    }
}
