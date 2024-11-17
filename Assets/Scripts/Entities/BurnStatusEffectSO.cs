using DG.Tweening;
using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Status Effect/Burn")]
public class BurnStatusEffectSO : StatusEffectSO
{
    [Header("Burn: Settings")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float tickDuration = 0.5f;
    private float tickTimer;

    private Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();

    private protected override void OnApply()
    {
        base.OnApply();

        SaveDefaultTints();
        TweenTintEntity();
    }

    public override void Update()
    {
        base.Update();

        tickTimer += Time.deltaTime;
        if(tickTimer > tickDuration)
        {
            tickTimer = 0;

            entity.TakeDamageWithoutState(damagePerTick, entity.GetColliderCenterPosition(), source);
        }
    }

    private protected override void OnExpire()
    {
        TweenUnTintEntity();

        base.OnExpire();
    }

    public override void Cancel()
    {
        ResetTint();

        base.Cancel();
    }

    private void SaveDefaultTints()
    {
        // Get all renderers in the character model, including any child objects
        Renderer[] renderers = entity.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            if (renderer.TryGetComponent(out Weapon weapon)) continue;

            Color[] colors = new Color[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors[i] = renderer.materials[i].color;
                }
            }
            originalColors.Add(renderer, colors);
        }
    }

    private void TweenTintEntity()
    {
        foreach (Renderer renderer in originalColors.Keys)
        {
            DOTween.Kill(renderer);
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.DOColor(Color.red, 0.5f);
                }
            }
        }
    }

    private void TweenUnTintEntity()
    {
        foreach (KeyValuePair<Renderer, Color[]> entry in originalColors)
        {
            Renderer renderer = entry.Key;
            Color[] colors = entry.Value;

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                DOTween.Kill(renderer);
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    renderer.materials[i].DOColor(colors[i], 0.5f);
                }
            }
        }
    }

    private void ResetTint()
    {
        foreach (KeyValuePair<Renderer, Color[]> entry in originalColors)
        {
            Renderer renderer = entry.Key;
            Color[] colors = entry.Value;

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                DOTween.Kill(renderer);
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    renderer.materials[i].color = colors[i];
                }
            }
        }
    }
}
