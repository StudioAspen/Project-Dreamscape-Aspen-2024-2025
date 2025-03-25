using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTextGlow : HoverUI
{
    private TMP_Text text;
    public static Material GlowMaterial { get; private set; }

    [Header("Config")]
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float glowPower = 1.5f;

    private Material originalMaterial;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        if (text == null) text = GetComponentInChildren<TMP_Text>();
        if(text == null)
        {
            Destroy(this);
            return;
        }
        originalMaterial = text.fontMaterial;
        GlowMaterial = new Material(originalMaterial);
        GlowMaterial.EnableKeyword("GLOW_ON"); // Enable the glow keyword
        GlowMaterial.SetColor(ShaderUtilities.ID_GlowColor, glowColor);
        GlowMaterial.SetFloat(ShaderUtilities.ID_GlowPower, glowPower);
    }

    private protected override void OnOnEnable()
    {
        text.fontMaterial = originalMaterial;
    }

    private protected override void OnOnSelected(BaseEventData eventData)
    {
        text.fontMaterial = GlowMaterial;
    }

    private protected override void OnOnDeselected(BaseEventData eventData)
    {
        text.fontMaterial = originalMaterial;
    }
}

