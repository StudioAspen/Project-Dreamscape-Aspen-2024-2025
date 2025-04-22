using UnityEngine;

public class UIPanel : MonoBehaviour
{
    private protected UIManager uiManager;
    private protected GameInputManager gameInputManager;

    [field: Header("Selection")]
    [field: SerializeField] public GameObject DefaultSelectedObject { get; private set; }
    private GameObject originalDefaultSelectedObject;

    /// <summary>
    /// This method is called on start inside the UIManager.
    /// </summary>
    /// <param name="uiManager"></param>
    /// <param name="gameInputManager"></param>
    public void Init(UIManager uiManager, GameInputManager gameInputManager)
    {
        this.uiManager = uiManager;
        this.gameInputManager = gameInputManager;

        originalDefaultSelectedObject = DefaultSelectedObject;
    }

    /// <summary>
    /// This method fires when the control scheme is changed to deselect the current panel.
    /// Override this method to implement custom deselection logic.
    /// </summary>
    public virtual void OnDeselected() { }

    /// <summary>
    /// Changes the default selected gameObject for switching control schemes at runtime.
    /// </summary>
    /// <param name="newObject">The new default selected object</param>
    public void ChangeDefaultSelectedObject(GameObject newObject) 
    {
        DefaultSelectedObject = newObject;
    }

    /// <summary>
    /// Restores the default selected gameObject to be the original one.
    /// </summary>
    public void RestoreDefaultSelectedObject()
    {
        DefaultSelectedObject = originalDefaultSelectedObject;
    }
}
