using UnityEngine;

public class GeyserCooldownState : GeyserBaseState
{
    [field: Header("Config")]
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
            geyser.ChangeState(geyser.GeyserIdleState);
            return;
        }
    }
}
