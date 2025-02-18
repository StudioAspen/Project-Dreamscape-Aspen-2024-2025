using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeGrowthState : SlimeBaseState
{
    [field: Header("Config")]

    // set to how long it take for the slime to grow
    [field: SerializeField] public float growthTimer { get; private set; } = 3f;
    [field: SerializeField] public float scaleIncrement {get; private set;} = 0.01f;

    private bool grew = false;
    private float currentScale = 0f;

    private float currentTime = 0f;

    public override void OnEnter()
    {
        currentTime = 0f;
        currentScale = 0.5f;

    }
    public override void OnExit()
    {
        
        base.OnExit();
    }

    public override void OnUpdate()
    {
        currentTime += Time.deltaTime;

        if (currentTime > growthTimer && grew == false)
        {
            StartCoroutine(scaleToOriginal());
        }
        else if (grew == true)
        {
            grew = false;
            slime.ChangeState(slime.SlimeWanderState);
        }
    }

    IEnumerator scaleToOriginal()
    {
        while (currentScale < slime.startScale)
        {
            currentScale += scaleIncrement;
            slime.transform.localScale = Vector3.one * currentScale;
            yield return null;
        }
        grew = true;
        slime.ChangeState(slime.SlimeWanderState);
    }
}
