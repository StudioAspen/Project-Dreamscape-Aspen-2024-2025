using UnityEngine;

public class LandBorder : MonoBehaviour
{
    [field: SerializeField] public Vector2Int RelativeBorderPosition { get; private set; }
    [field: SerializeField] public Vector2Int WorldBorderPosition { get; private set; }

    public void SetWorldBorderPosition(Vector2Int landWorldPos)
    {
        WorldBorderPosition = landWorldPos + RelativeBorderPosition;
    }
}
