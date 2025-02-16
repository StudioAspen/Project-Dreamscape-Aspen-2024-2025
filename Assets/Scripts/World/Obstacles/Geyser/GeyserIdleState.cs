using UnityEngine;

public class GeyserIdleState : GeyserBaseState
{
    [field: Header("Config")]
    [field: SerializeField] public Vector2 DurationRange { get; private set; } = new Vector2(3f, 15f);
    private float timer;
    private float randomDuration;

    public override void OnEnter()
    {
        timer = 0f;
        randomDuration = Random.Range(DurationRange.x, DurationRange.y);
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if(timer > randomDuration)
        {
            geyser.ChangeState(geyser.GeyserWarningState);
            return;
        }
    }
}
