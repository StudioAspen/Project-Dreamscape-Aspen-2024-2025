using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuff : MonoBehaviour
{
    public string StatAffected { get; private set; }
    public float ModifierAmount { get; private set; }
    public float Duration { get; private set; }
    public bool IsDebuff { get; private set; }
    private float timeRemaining;

    public PlayerBuff(string statAffected, float modifierAmount, float duration, bool isDebuff = false)
    {
        StatAffected = statAffected;
        ModifierAmount = modifierAmount;
        Duration = duration;
        IsDebuff = isDebuff;
        timeRemaining = duration;
    }

    public void StartBuff()
    {
        timeRemaining = Duration;
        StartCoroutine(BuffDuration());
    }

    private IEnumerator BuffDuration()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            yield return null; 
        }
        OnBuffExpired();
    }

    private void OnBuffExpired()
    {
        Debug.Log($"Buff expired: {StatAffected}");
    }

    public bool IsExpired()
    {
        return timeRemaining <= 0;
    }

    public void UpdateTime(float deltaTime)
    {
        timeRemaining -= deltaTime;
    }
}

