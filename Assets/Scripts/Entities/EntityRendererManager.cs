using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EntityRendererManager : MonoBehaviour
{
    [field: SerializeField] public List<Renderer> Renderers { get; private set; } = new List<Renderer>();

    private Dictionary<Renderer, List<Material>> originalMaterials = new Dictionary<Renderer, List<Material>>();
    private Dictionary<Renderer, List<Color>> originalColors = new Dictionary<Renderer, List<Color>>();

/*    [field: SerializeField, SerializedDictionary("Biome", "Texture")]
    public SerializedDictionary<Biome, Texture> BiomeTextures { get; private set; } = new();
*/
    private void Awake()
    {
        CacheOriginalMaterials();
        CacheOriginalTints();
    }

    #region Material Functions
    /// <summary>
    /// Caches the original materials of the entity's mesh renderers.
    /// </summary>
    public void CacheOriginalMaterials()
    {
        foreach (Renderer renderer in Renderers)
        {
            List<Material> materials = new List<Material>();
            renderer.GetSharedMaterials(materials);

            originalMaterials.Add(renderer, materials);
        }
    }

    /// <summary>
    /// Adds a new material to the entity's mesh renderers.
    /// </summary>
    /// <param name="newMaterial">The new material to add.</param>
    public void AddMaterial(Material newMaterial)
    {
        foreach (Renderer renderer in Renderers)
        {
            List<Material> materials = new List<Material>();
            renderer.GetSharedMaterials(materials);
            materials.Add(newMaterial);
            renderer.SetSharedMaterials(materials);
        }
    }

    /// <summary>
    /// Removes a material from the entity's mesh renderers.
    /// </summary>
    /// <param name="material">The material to remove.</param>
    public void RemoveMaterial(Material material)
    {
        foreach (Renderer renderer in Renderers)
        {
            List<Material> materials = new List<Material>();
            renderer.GetSharedMaterials(materials);
            if (materials.Contains(material)) materials.Remove(material);
            else
            {
                Debug.LogWarning($"Material {material.name} not found in {renderer.name}.");
                continue;
            }
            renderer.SetSharedMaterials(materials);
        }
    }

    /// <summary>
    /// Removes all materials from the entity's mesh renderers.
    /// </summary>
    public void RemoveAllMaterials()
    {
        foreach (Renderer renderer in Renderers)
        {
            renderer.SetSharedMaterials(new List<Material>());
        }
    }

    /// <summary>
    /// Restores the original materials of the entity's mesh renderers.
    /// </summary>
    public void RestoreOriginalMaterials()
    {
        foreach (Renderer renderer in originalMaterials.Keys)
        {
            renderer.SetSharedMaterials(originalMaterials[renderer]);
        }
    }
    #endregion

    #region Tinting Functions
    /// <summary>
    /// Caches the original tints of the renderers in the character model.
    /// </summary>
    private void CacheOriginalTints()
    {
        foreach (Renderer renderer in Renderers)
        {
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
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
        foreach (KeyValuePair<Renderer, List<Color>> entry in originalColors)
        {
            Renderer renderer = entry.Key;
            List<Color> colors = entry.Value;

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
        foreach (KeyValuePair<Renderer, List<Color>> entry in originalColors)
        {
            Renderer renderer = entry.Key;
            List<Color> colors = entry.Value;

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