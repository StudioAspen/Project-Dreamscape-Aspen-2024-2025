using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Fear/Passive B")]
public class AspectOfFearPassiveBStatusEffectSO : StatusEffectSO
{
    [field: Header("Aspect of Fear Passive B: Settings")]
    [field: SerializeField] public int MaxStacks { get; private set; }
    [field: SerializeField] public float StackTimerReset { get; private set; }
    [field: SerializeField] public float SpeedModifierBuff { get; private set; }
    [field: SerializeField] public float DamageModifierBuff { get; private set; }
    private int currentStacks;
    private float timer;

    private float originalSpeed;
    private Stat originalDamage;

    private HashSet<string> activeBuffs = new HashSet<string>(); // checks for current active buffs, so non-active buffs can be activated when stacks are filled again

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();
        originalSpeed = entity.SpeedModifier;
    }

    public override void Update()
    {
        // cheat to add stacks
        if (Input.GetKeyDown(KeyCode.P))
            ApplyStack();

        base.Update();

        // increment timer for stacks
        timer += entity.LocalDeltaTime;

        // functionality for when max stacks are reached
        if (currentStacks == MaxStacks)
        {
            Debug.Log("max stacks");

            // create a list of non-active buffs that are not currently active
            List<string> nonActiveBuffs = new List<string>();
            if (!activeBuffs.Contains("SpeedBoost")) nonActiveBuffs.Add("SpeedBoost");
            if (!activeBuffs.Contains("DamageIncrease")) nonActiveBuffs.Add("DamageIncrease");

            // select a random buff from the available list
            string selectedBuff = nonActiveBuffs[Random.Range(0, nonActiveBuffs.Count)];

            // apply the selected buff and mark it as active
            // then refresh timer, stacks, and duration of buffs
            if (selectedBuff == "SpeedBoost")
            {
                entity.SetSpeedModifier(SpeedModifierBuff);
                activeBuffs.Add("SpeedBoost");
                Debug.Log("SPEED boost from aspect of fear");

                timer = 0f;
                currentStacks = 0;
            }
            else if (selectedBuff == "DamageIncrease")
            {
                //entity.DamageModifier.AddFlatAmount(DamageModifierBuff, this); // flat amount
                entity.DamageModifier.AddMultiplier(DamageModifierBuff, this); // multiplier
                activeBuffs.Add("DamageIncrease");
                Debug.Log("DAMAGE boost from aspect of fear");

                timer = 0f;
                currentStacks = 0;
            }

            // if all buffs are active, reset timer for stack to reset
            timer = 0f;
            currentStacks = 0;
        }

        // when stack timer is reached, refresh timer, stacks, active buffs list, and buffs
        if (timer > StackTimerReset)
        {
            Debug.Log("reset BUFFS");
            timer = 0f;
            currentStacks = 0;
            activeBuffs.Clear();

            entity.SetSpeedModifier(originalSpeed);
            //entity.DamageModifier.AddFlatAmount(originalDamage, this); // flat amount
            entity.DamageModifier.RemoveMultiplier(DamageModifierBuff, this); // multiplier
        }
    }

    private void ApplyStack()
    {
        // reset timer and add stack
        timer = 0f;
        currentStacks++;
        Debug.Log("STACKS = " + currentStacks);
    }
}
