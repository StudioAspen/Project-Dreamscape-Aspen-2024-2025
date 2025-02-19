using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamBushHostileState : DreamBushBaseState
{

    [field: Header("Config")]
    [field: SerializeField] public float Duration { get; private set; } = 1f;
    private float timer;

    public override void OnEnter()
    {
        timer = 0;
        dreamBush.Renderer.material.color = Color.red;
    }

    public override void OnExit()
    {
        dreamBush.Renderer.material.color = Color.white;
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer > Duration)
        {
            dreamBush.ChangeState(dreamBush.DreamBushNeutralState);
            return;
        }
    }
}
