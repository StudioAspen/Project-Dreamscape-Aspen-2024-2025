using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Fear/Passive B")]
public class AspectOfFearPassiveBStatusEffectSO : StatusEffectSO
{
    [field: Header("Aspect of Fear Passive B: Settings")]
    [field: SerializeField] public int MaxStacks { get; private set; }
    [field: SerializeField] public float StackTimerReset { get; private set; }
    [field: SerializeField] public float SpeedModifierBuff { get; private set; }
    private int currentStacks;
    private float timer;

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
        // cheat to add stacks
        if (Input.GetKeyDown(KeyCode.P))
            ApplyStack();

        base.Update();

        // increment timer for stacks
        timer += entity.LocalDeltaTime;
        //Debug.Log(timer);

        // functionality for when max stacks are reached
        if (currentStacks == MaxStacks)
        {
            Debug.Log("max stacks");
            // apply buffs for player
            entity.SetSpeedModifier(SpeedModifierBuff);

            // reset stacks and timer
            timer = 0f;
            currentStacks = 0;
        }
        
        // when stack timer is reached reset stacks
        if (timer > StackTimerReset)
        {
            timer = 0f;
            currentStacks = 0;
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
