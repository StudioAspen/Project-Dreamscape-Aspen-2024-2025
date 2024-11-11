using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StatusEffector : ScriptableObject
{
    [field:SerializeField]public Entity.EntityStats StatAffected { get; private set; }
    [field: SerializeField] public float ModifierAmount { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public bool IsDebuff { get; private set; }
    private float timeRemaining;

    public StatusEffector(Entity.EntityStats statAffected, float modifierAmount, float duration, bool isDebuff = false)
    {
        StatAffected = statAffected;
        ModifierAmount = modifierAmount;
        Duration = duration;
        IsDebuff = isDebuff;
        timeRemaining = duration;
    }

    public void StartEffect()
    {
        timeRemaining = Duration;
    }
    public void UpdateTime(float deltaTime)
    {
        timeRemaining -= deltaTime;
    }

    private IEnumerator EffectDuration()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        OnEffectExpired();
    }

    public bool IsExpired()
    {
        return timeRemaining <= 0;
    }

    private void OnEffectExpired()
    {
        Debug.Log($"Buff expired: {StatAffected}");
    }

    

    
}
