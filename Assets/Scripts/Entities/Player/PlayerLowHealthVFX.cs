using System;
using UnityEngine;

public class PlayerLowHealthVFX : MonoBehaviour
{
    private Player player;

    [Header("References")]
    [SerializeField] private Material lowHealthVFX;

    [Header("Config")]
    [SerializeField] private float lowHealthThreshold = 0.25f;

    private const string COLOR_PROPERTY = "_Color";
    private const string SIZE_PROPERTY = "_Size";
    private const string SCALE_PROPERTY = "_Scale";
    private const string SPEED_PROPERTY = "_Speed";
    private const string CONTRAST_PROPERTY = "_Contrast";
    private const string ACTIVE_PROPERTY = "_ACTIVE";

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        player.OnEntityTakeDamage += Player_OnEntityTakeDamage;
        player.OnEntityHeal += Player_OnEntityHeal;

        OnHealthChanged();
    }

    private void OnDisable()
    {
        player.OnEntityTakeDamage -= Player_OnEntityTakeDamage;
        player.OnEntityHeal -= Player_OnEntityHeal;

        lowHealthVFX.DisableKeyword(ACTIVE_PROPERTY);
    }

    private void Player_OnEntityHeal(Entity entity, int healAmount)
    {
        OnHealthChanged();
    }

    private void Player_OnEntityTakeDamage(int damage, Vector3 hitPoint, GameObject source)
    {
        OnHealthChanged();
    }

    private void OnHealthChanged()
    {
        float healthPercent = player.CurrentHealth / player.MaxHealth.GetFloatValue();

        if(healthPercent < lowHealthThreshold)
        {
            lowHealthVFX.EnableKeyword(ACTIVE_PROPERTY);
        }
        else
        {
            lowHealthVFX.DisableKeyword(ACTIVE_PROPERTY);
        }
    }
}
