using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggYolkCooldownState : MonoBehaviour
{
    [field SerializeField] public float Duration { get; private set; } = 1f;
    private float timer;

    public override OnEnter()
    {
        timer = 0f;
    }

    public override OnExit()
    {

    }

    public override OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer > Duration)
        {
            eggYolk.ChangeState(eggYolk.EggYolkIdleState)
            return;
        }
    }
}
