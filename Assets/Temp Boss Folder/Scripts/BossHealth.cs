using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public bool IsInvulnerable { get; private set; }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsInvulnerable) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void SetInvulnerable(bool state)
    {
        IsInvulnerable = state;
    }

    void Die()
    {
        Debug.Log("Boss Defeated");
        Destroy(gameObject);
    }
}
