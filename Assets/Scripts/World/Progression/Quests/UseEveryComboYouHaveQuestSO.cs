using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Use Every Combo You Have Quest", menuName = "World/Progression Quest/Use Every Combo You Have")]
public class UseEveryComboYouHaveQuestSO : ProgressionQuestSO
{
    private ChainingSystem chainingSystem;
    private Weapon weapon;
    private List<ComboDataSO> combos;
    private HashSet<ComboDataSO> combosExecuted;

    private protected override void OnActivated()
    {
        chainingSystem = FindObjectOfType<ChainingSystem>();
        weapon = FindObjectOfType<Weapon>();
        combos = weapon.Combos;
        combosExecuted = new HashSet<ComboDataSO>();
    }

    private protected override void OnCleanUp()
    {
        throw new System.NotImplementedException();
    }

    private protected override void OnUpdate()
    {
        
    }
}
