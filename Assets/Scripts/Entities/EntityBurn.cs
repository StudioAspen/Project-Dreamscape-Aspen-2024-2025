using DG.Tweening;
using KBCore.Refs;
using System.Collections.Generic;
using UnityEngine;

public class EntityBurn : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Self] private Entity entity;

    [Header("Settings")]
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float tickDuration = 0.5f;
    private int currentTicks;
    private float tickTimer;

    private GameObject source;

    private Dictionary<Renderer, Color[]> originalColors = new Dictionary<Renderer, Color[]>();
    private bool isTinted;

    private void OnValidate()
    {
        this.ValidateRefs();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        UnTintEntity();
    }

    private void Start()
    {
        SaveDefaultTints();
    }

    private void Update()
    {
        HandleTick();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddBurn(3.5f, entity.gameObject);
        }
    }

    private void HandleTick()
    {
        if (currentTicks <= 0)
        {
            tickTimer = 0f;

            if (isTinted) UnTintEntity();            
            return;
        }

        //Debug.Log("Burning");

        tickTimer += Time.deltaTime;
        if(tickTimer > tickDuration)
        {
            tickTimer = 0;
            OnTick();
        }
    }

    private void OnTick()
    {
        currentTicks--;

        entity.TakeDamageWithoutState(damagePerTick, entity.GetColliderCenterPosition(), source);
    }

    public void AddBurn(float duration, GameObject source)
    {
        this.source = source;
        currentTicks += Mathf.FloorToInt(duration / tickDuration);

        TintEntity();
    }

    private void SaveDefaultTints()
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

    private void TintEntity()
    {
        isTinted = true;
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

    private void UnTintEntity()
    {
        isTinted = false;
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
}
