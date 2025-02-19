using UnityEngine;

[System.Serializable]
public class EntityDeathState : EntityBaseState
{
    [field: SerializeField] public AnimationClip AnimationClip { get; private set; }

    public override void OnEnter()
    {
        entity.PlayOneShotAnimation(AnimationClip);

        entity.SetSpeedModifier(0f);

        entity.LocalTimeScale.ClearMultipliers();

        entity.IgnoreOtherEntityCollisions(true);
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        entity.ApplyGravity();
    }
}
