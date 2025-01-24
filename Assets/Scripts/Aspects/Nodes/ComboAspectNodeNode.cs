using UnityEngine;

public class ComboAspectNodeNode : AspectNodeNode
{
    [field: Header("Combo")]
    [field: SerializeField] public ComboDataSO ComboData { get; private set; }

    public override void ApplyAspect(AspectsManager aspectsManager)
    {
        base.ApplyAspect(aspectsManager);

        if (ComboData == null)
        {
            Debug.LogError($"No ComboData found in {name}");
            return;
        }

        Weapon ownerWeapon = aspectsManager.GetComponentInChildren<Weapon>();
        if (ownerWeapon == null)
        {
            Debug.LogError($"No weapon found in children of {aspectsManager.gameObject.name}");
            return;
        }
        
        ownerWeapon.AddCombo(ComboData);
    }
}
