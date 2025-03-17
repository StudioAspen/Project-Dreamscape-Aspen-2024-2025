using System;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial World Event", menuName = "World/World Event/Tutorial")]
public class TutorialWorldEventSO : WorldEventSO
{
    private Player player;
    private Weapon hammer;

    [field: Header("Config")]
    [field: SerializeField] public Dummy DummyPrefab { get; private set; }
    private Dummy dummyInstance;

    private HashSet<ComboDataSO> remainingCombos = new();
    private ComboDataSO currentCombo;
    private int totalCombos;

    private protected override void OnStarted()
    {
        player = FindObjectOfType<Player>();
        hammer = player.GetComponentInChildren<Weapon>();

        remainingCombos = new HashSet<ComboDataSO>(hammer.Combos);
        totalCombos = remainingCombos.Count;
        Debug.Log($"Remaining Combos: {GetRemainingCombos()}");

        hammer.OnWeaponHit += Hammer_OnWeaponHit;
        hammer.OnWeaponStartSwing += Hammer_OnWeaponStartSwing;

        dummyInstance = Instantiate(DummyPrefab, 5f * Vector3.up, Quaternion.identity);
    }

    private protected override void OnCleared()
    {
        hammer.OnWeaponHit -= Hammer_OnWeaponHit;
        hammer.OnWeaponStartSwing -= Hammer_OnWeaponStartSwing;

        if(dummyInstance != null) dummyInstance.Die();
    }

    private protected override void OnUpdate()
    {
        
    }

    public override void UpdateEventUIElements(TMP_Text feedbackText, TMP_Text nameText)
    {
        feedbackText.text = $"{totalCombos - remainingCombos.Count}/{totalCombos}";
        nameText.text = $"{EventProgressionUIName.ToUpper()}";
    }

    private void Hammer_OnWeaponHit(Entity attacker, Entity victim, Vector3 hitPoint, int damage)
    {
        if (currentCombo == null) return;
        if (!remainingCombos.Contains(currentCombo)) return;

        remainingCombos.Remove(currentCombo);
        Debug.Log($"Remaining Combos: {GetRemainingCombos()}");

        if (remainingCombos.Count <= 0)
        {
            eventManager.ClearEvent();
            return;
        }
    }

    private void Hammer_OnWeaponStartSwing(Entity entity, ComboDataSO combo)
    {
        currentCombo = combo;
    }

    private string GetRemainingCombos()
    {
        string result = "";
        foreach (var combo in remainingCombos)
        {
            result += $"{combo.name}, ";
        }
        return result;
    }
}
