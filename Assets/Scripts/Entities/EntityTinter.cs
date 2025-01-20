using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EntityTinter : MonoBehaviour
{
    private Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();

    private void Awake()
    {
        CacheOriginalTints();
    }

    #region Tinting Functions
    /// <summary>
    /// Caches the original tints of the renderers in the character model.
    /// </summary>
    private void CacheOriginalTints()
    {
        // Get all renderers in the character model, including any child objects
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

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

    /// <summary>
    /// Tints the entity with the specified new color.
    /// </summary>
    /// <param name="newColor">The new color to apply.</param>
    public void Tint(Color newColor)
    {
        foreach (Renderer renderer in originalColors.Keys)
        {
            DOTween.Kill(renderer);
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = newColor;
                }
            }
        }
    }

    /// <summary>
    /// Tweens the object's tint color to the specified new color.
    /// </summary>
    /// <param name="newColor">The new color to tween to.</param>
    /// <param name="duration">The duration of the tween</param>
    public void TweenTint(Color newColor, float duration = 0.2f)
    {
        foreach (Renderer renderer in originalColors.Keys)
        {
            DOTween.Kill(renderer);
            foreach (Material material in renderer.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.DOColor(newColor, duration);
                }
            }
        }
    }

    /// <summary>
    /// Tweens the entity back to its original colors.
    /// </summary>
    /// <param name="duration">The duration of the tween</param>
    public void TweenUnTint(float duration = 0.2f)
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
                    renderer.materials[i].DOColor(colors[i], duration);
                }
            }
        }
    }

    /// <summary>
    /// Immediately resets the tint of the entity to its original colors.
    /// </summary>
    public void ResetTint()
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
    #endregion
}