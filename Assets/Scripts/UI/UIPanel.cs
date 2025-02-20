using UnityEngine;

public class UIPanel : MonoBehaviour
{
    [field: Header("Selection")]
    [field: SerializeField] public GameObject DefaultSelectedObject { get; private set; }

    /// <summary>
    /// This method fires when the control scheme is changed to deselect the current panel.
    /// Override this method to implement custom deselection logic.
    /// </summary>
    public virtual void OnDeselected() { }
}
