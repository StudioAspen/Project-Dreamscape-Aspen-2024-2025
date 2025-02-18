using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Fear/Passive B")]
public class AspectOfFearPassiveBStatusEffectSO : StatusEffectSO
{
    [field: Header("Aspect of Fear Passive B: Settings")]
    [field: SerializeField] public int MaxStacks { get; private set; } = 5;
    [field: SerializeField] public float SpeedModifierBuff { get; private set; } = 5;
    private int currentStacks;

    private void OnValidate()
    {
        Stackable = true; // force stackable otherwise override wont work
    }

    private protected override void OnApply()
    {
        base.OnApply();
    }

    public override void Update()
    {
        base.Update();

        // cheat to add stacks
        if (Input.GetKeyDown(KeyCode.P))
        {
            ApplyStack();
        }

        // functionality for when max stacks are reached
        if (currentStacks == MaxStacks)
        {
            Debug.Log("max stacks");
            // apply buffs for player
            entity.SetSpeedModifier(SpeedModifierBuff);

            // reset stacks
            currentStacks = 0;
        }
    }

    private void ApplyStack()
    {
        currentStacks++;
        Debug.Log("STACKS +1");
    }
}
