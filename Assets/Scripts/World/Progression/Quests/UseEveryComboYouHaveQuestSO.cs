using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Use Every Combo You Have Quest", menuName = "World/Progression Quest/Use Every Combo You Have")]
public class UseEveryComboYouHaveQuestSO : ProgressionQuestSO
{
    private Player player;
    private Weapon weapon;
    private List<ComboDataSO> combos;
    private HashSet<ComboDataSO> combosExecuted;

    private protected override void OnActivated()
    {
        player = FindObjectOfType<Player>();
        weapon = FindObjectOfType<Weapon>();
        combos = weapon.Combos;
        combosExecuted = new HashSet<ComboDataSO>();
    }

    private protected override void OnCleanUp()
    {
        
    }

    private protected override void OnUpdate()
    {
        currentCombo = player.PlayerAttackState.ComboData;

        if (!combosExecuted.Contains(currentCombo))
        {
            combosExecuted.Add(currentCombo);
        }

        if (combosExecuted.Count == combos.Count)
        {
            Complete();
        }
    }
}
