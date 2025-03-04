using UnityEngine;

[System.Serializable]
public class EntityDeathState : EntityBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }
    [field: SerializeField] public float DeathDuration { get; private set; } = 1f;
    private protected float timer;

    private protected bool isDestroyed;

    public override void OnEnter()
    {
        entity.PlayOneShotAnimation(AnimationClip, DeathDuration);

        entity.SetSpeedModifier(0f);

        entity.LocalTimeScale.ClearAllBuffs();

        entity.IgnoreOtherEntityCollisions(true);

        timer = 0f;
        isDestroyed = false;
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        entity.ApplyGravity();

        timer += entity.LocalDeltaTime;
        if(timer > DeathDuration && !isDestroyed)
        {
            isDestroyed = true;
            entity.Die();
            return;
        }
    }
}
