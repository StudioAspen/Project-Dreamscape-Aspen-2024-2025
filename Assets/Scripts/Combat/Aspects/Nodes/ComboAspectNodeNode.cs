using UnityEngine;

public class ComboAspectNodeNode : AspectNodeNode
{
    [field: Header("Combo")]
    [field: SerializeField] public ComboDataSO ComboData { get; private set; }

    public override void ApplyAspect(AspectsManager aspectsManager)
    {
        base.ApplyAspect(aspectsManager);

        Weapon ownerWeapon = aspectsManager.GetComponentInChildren<Weapon>();
        if (ownerWeapon == null)
        {
            Debug.LogError($"No weapon found in children of {aspectsManager.gameObject.name}");
        }
        
        ownerWeapon.AddCombo(ComboData);
    }
}
