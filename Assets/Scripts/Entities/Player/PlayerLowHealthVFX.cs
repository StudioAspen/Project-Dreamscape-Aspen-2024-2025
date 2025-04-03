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

    private void Update()
    {
        // float healthPercent = player.CurrentHealth / player.MaxHealth.GetFloatValue();

        if (Input.GetKeyDown(KeyCode.V))
        {
            lowHealthVFX.SetFloat(ACTIVE_PROPERTY, 1.0f);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            lowHealthVFX.SetFloat(ACTIVE_PROPERTY, 0f);
        }
    }
}
