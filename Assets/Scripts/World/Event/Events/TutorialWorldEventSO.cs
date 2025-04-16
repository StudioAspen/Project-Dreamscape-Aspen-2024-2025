using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [field: SerializeField] public float AfterFinishDelay { get; private set; } = 10f;
    private Dummy dummyInstance;

    private HashSet<ComboDataSO> remainingCombos = new();
    private ComboDataSO currentCombo;
    private ComboDataSO nextCombo;
    private int totalCombos;

    private bool isFinished;
    private float afterFinishTimer;

    private TMP_Text optionalDescriptionTextReference;

    private protected override void OnStarted()
    {
        player = FindObjectOfType<Player>();
        hammer = player.GetComponentInChildren<Weapon>();

        remainingCombos = new HashSet<ComboDataSO>(hammer.Combos);
        totalCombos = remainingCombos.Count;
        nextCombo = remainingCombos.ToList()[0];

        hammer.OnWeaponHit += Hammer_OnWeaponHit;
        hammer.OnWeaponStartSwing += Hammer_OnWeaponStartSwing;

        dummyInstance = Instantiate(DummyPrefab, new Vector3(worldManager.LandScale/2f, 5f, worldManager.LandScale/2f), Quaternion.Euler(0f, 180f, 0f));

        isFinished = false;
        afterFinishTimer = 0f;
    }

    private protected override void OnCleared()
    {
        hammer.OnWeaponHit -= Hammer_OnWeaponHit;
        hammer.OnWeaponStartSwing -= Hammer_OnWeaponStartSwing;

        if(dummyInstance != null) dummyInstance.Die();

        optionalDescriptionTextReference.text = "";
    }

    private protected override void OnUpdate()
    {
        if (isFinished)
        {
            afterFinishTimer += Time.deltaTime;
            if(afterFinishTimer > AfterFinishDelay)
            {
                eventManager.ClearEvent();
                return;
            }
        }
    }

    public override void UpdateEventUIElements(TMP_Text feedbackText, TMP_Text nameText, TMP_Text optionalDescriptionText)
    {
        feedbackText.text = isFinished ? $"{GetFormattedFloatTimer(AfterFinishDelay - afterFinishTimer)}" : $"{totalCombos - remainingCombos.Count}/{totalCombos}";
        nameText.text = $"{EventProgressionUIName.ToUpper()}";
        optionalDescriptionTextReference = optionalDescriptionText;

        if (isFinished)
        {
            optionalDescriptionText.text = $"Tutorial completed, good luck on your journey dreamer!\nNew enemies, abilities, and lands await you...";
        }
        else
        {
            string inputsDescription = "";
            for (int i = 0; i < nextCombo.ComboInputs.Count; i++)
            {
                string comboString = FirstLetterToUpperOthersLower(nextCombo.ComboInputs[i].ToString());
                if (nextCombo.IsAirCombo) comboString = $"Air {comboString}";
                if (i == 0) inputsDescription += $"{comboString}";
                else inputsDescription += $"->{comboString}";
            }
            optionalDescriptionText.text = $"Perform combo on Dummy: {inputsDescription}";
        }
    }

    private void Hammer_OnWeaponHit(Entity attacker, Entity victim, Vector3 hitPoint, int damage)
    {
        if (currentCombo == null) return;
        if (remainingCombos.Count == 0) return;
        if (currentCombo != nextCombo) return;
        if (!remainingCombos.Contains(currentCombo)) return;

        remainingCombos.Remove(currentCombo);
        if(remainingCombos.Count > 0) nextCombo = remainingCombos.ToList()[0];

        if(remainingCombos.Count <= 0)
        {
            isFinished = true;
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
