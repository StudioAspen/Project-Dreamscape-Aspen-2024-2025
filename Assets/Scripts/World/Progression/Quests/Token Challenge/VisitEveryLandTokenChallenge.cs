using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Achieve Chain Count Progression Quest", menuName = "World/Progression Quest/Visit Every Land During Wave Token Challenge")]
public class VisitEveryLandTokenChallenge : ProgressionQuestSO
{
    private ChainingSystem chainingSystem;

    [field: Header("Config")]
    [field: SerializeField] public int ChainGoal { get; private set; } = 15;

    private Dictionary<Vector2Int, GameObject> visitIndicatorsDictionary = new Dictionary<Vector2Int, GameObject>();

    private protected WorldManager worldManager;

    private protected override void OnActivated()
    {
        chainingSystem = FindObjectOfType<ChainingSystem>();
        if (chainingSystem == null)
        {
            CleanUp();
            return;
        }

        // create visit indicators on all lands
        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            visitIndicatorsDictionary.Add(land.GridPosition,
                CustomDebug.InstantiateTemporarySphere(land.transform.position + 5f * Vector3.up, 10f, Mathf.Infinity, new Color(1, 0, 0, 0.5f)));
        }
    }

    private protected override void OnCleanUp()
    {

    }

    private protected override void OnUpdate()
    {
        if (chainingSystem == null) return; ;

        if (chainingSystem.ChainCount >= ChainGoal)
        {
            Complete();
            return;
        }
    }
}
