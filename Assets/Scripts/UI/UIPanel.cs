using UnityEngine;

public class UIPanel : MonoBehaviour
{
    private protected UIManager uiManager;
    private protected GameInputManager gameInputManager;

    [field: Header("Selection")]
    [field: SerializeField] public GameObject DefaultSelectedObject { get; private set; }

    /// <summary>
    /// This method is called on start inside the UIManager.
    /// </summary>
    /// <param name="uiManager"></param>
    /// <param name="gameInputManager"></param>
    public void Init(UIManager uiManager, GameInputManager gameInputManager)
    {
        this.uiManager = uiManager;
        this.gameInputManager = gameInputManager;
    }

    /// <summary>
    /// This method fires when the control scheme is changed to deselect the current panel.
    /// Override this method to implement custom deselection logic.
    /// </summary>
    public virtual void OnDeselected() { }
}
