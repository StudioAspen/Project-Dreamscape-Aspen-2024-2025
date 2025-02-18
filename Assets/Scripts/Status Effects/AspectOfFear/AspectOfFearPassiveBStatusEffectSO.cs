using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Aspect of Fear/Passive B")]
public class AspectOfFearPassiveBStatusEffectSO : StatusEffectSO
{
    [field: Header("Aspect of Fear Passive B: Settings")]
    [field: SerializeField] public int MaxStacks { get; private set; } = 5;
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            ApplyStack();
        }

        if (currentStacks == MaxStacks)
        {
            Debug.Log("max stacks");
        }
    }

    private void ApplyStack()
    {
        currentStacks++;
        Debug.Log("STACKS +1");
    }
}
