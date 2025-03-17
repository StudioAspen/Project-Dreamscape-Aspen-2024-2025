using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial World Event", menuName = "World/World Event/Tutorial")]
public class TutorialWorldEventSO : WorldEventSO
{
    private Player player;
    private Weapon hammer;

    [field: Header("Config")]
    [field: SerializeField] public int DummyIntVariable { get; private set; } = 2;

    private HashSet<ComboDataSO> remainingCombos = new();
    private int totalCombos;

    private protected override void OnStarted()
    {
        player = FindObjectOfType<Player>();
        hammer = player.GetComponentInChildren<Weapon>();

        remainingCombos = new HashSet<ComboDataSO>(hammer.Combos);
        totalCombos = remainingCombos.Count;
        Debug.Log($"Remaining Combos: {GetRemainingCombos()}");

        hammer.OnWeaponEndSwing += Hammer_OnWeaponEndSwing;
    }

    private protected override void OnCleared()
    {
        hammer.OnWeaponEndSwing -= Hammer_OnWeaponEndSwing;
    }

    private protected override void OnUpdate()
    {
        
    }

    public override void UpdateEventUIElements(TMP_Text feedbackText, TMP_Text nameText)
    {
        feedbackText.text = $"{totalCombos - remainingCombos.Count}/{totalCombos}";
        nameText.text = $"{EventProgressionUIName.ToUpper()}";
    }

    private void Hammer_OnWeaponEndSwing(Entity entity, ComboDataSO combo)
    {
        if(!remainingCombos.Contains(combo)) return;

        remainingCombos.Remove(combo);
        Debug.Log($"Remaining Combos: {GetRemainingCombos()}");

        if(remainingCombos.Count <= 0)
        {
            eventManager.ClearEvent();
            return;
        }
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
