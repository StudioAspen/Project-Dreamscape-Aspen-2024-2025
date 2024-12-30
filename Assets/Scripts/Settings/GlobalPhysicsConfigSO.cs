using UnityEngine;

[CreateAssetMenu(fileName = "Global Physics Config", menuName = "Configs/Global Physics Config")]
public class GlobalPhysicsConfigSO : ScriptableObject
{
    [field: SerializeField] public float Gravity { get; private set; } = -20f;
    [field: SerializeField] public float GroundedYVelocity { get; private set; } = -5f;
    [field: SerializeField] public float FallingStartingYVelocity { get; private set; } = 0f;
    [field: SerializeField] public LayerMask GroundLayer { get; private set; }
}