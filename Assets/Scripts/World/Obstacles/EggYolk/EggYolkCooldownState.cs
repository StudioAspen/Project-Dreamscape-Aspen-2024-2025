using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggYolkCooldownState : EggYolkBaseState
{
    [field: SerializeField] public float Duration { get; private set; } = 1f;
    private float timer;

    public override void OnEnter()
    {
        timer = 0f;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer > Duration)
        {
            eggYolk.ChangeState(eggYolk.EggYolkIdleState);
            return; 
        }
    }
}
