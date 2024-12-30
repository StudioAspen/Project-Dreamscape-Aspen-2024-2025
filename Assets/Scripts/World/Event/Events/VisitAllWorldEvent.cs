using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// All lands will light up. When the player steps on a land it will go away. All lands will spawn enemies.
// Once all the lands have been touched by the player, trigger EOW
public class VisitAllWorldEvent : BaseState
{
    private EventManager eventManager;
    private WorldManager worldManager;
    private EventsConfigSO eventsConfigSO;

    private List<Player> players = new List<Player>();

    private List<Coroutine> enemySpawningCoroutines = new List<Coroutine>();
    private Dictionary<Vector2Int, GameObject> visitIndicatorsDictionary = new Dictionary<Vector2Int, GameObject>();

    public VisitAllWorldEvent(EventManager eventManager, WorldManager worldManager, EventsConfigSO eventsConfigSO)
    {
        this.eventManager = eventManager;
        this.worldManager = worldManager;
        this.eventsConfigSO = eventsConfigSO;
    }

    public override void OnEnter()
    {
        players = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None).ToList();
        if(players == null)
        {
            eventManager.ClearEvent();
            return;
        }
        if(players.Count <= 0)
        {
            eventManager.ClearEvent();
            return;
        }

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            EnemySpawner enemySpawner = land.EnemySpawner;
            enemySpawningCoroutines.Add(eventManager.StartCoroutine(enemySpawner.SpawnWithCurrencyCoroutine()));

            visitIndicatorsDictionary.Add(land.GridPosition,
                CustomDebug.InstantiateTemporarySphere(land.transform.position + 5f * Vector3.up, 10f, Mathf.Infinity, new Color(1, 0, 0, 0.5f)));
        }
    }

    public override void OnExit()
    {
        foreach (Coroutine coroutine in enemySpawningCoroutines)
        {
            if (coroutine != null) eventManager.StopCoroutine(coroutine);
        }
        enemySpawningCoroutines.Clear();

        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.EnemySpawner.KillAll();
        }

        foreach (GameObject sphere in visitIndicatorsDictionary.Values)
        {
            GameObject.Destroy(sphere);
        }
        visitIndicatorsDictionary.Clear();
    }

    public override void Update()
    {
        if (visitIndicatorsDictionary.Count <= 0)
        {
            eventManager.ClearEvent();
            return;
        }

        for (int i = 0; i < players.Count; i++)
        {
            Vector2Int playerGridPosition = worldManager.GetGridPosition(players[i].transform.position);

            if (visitIndicatorsDictionary.ContainsKey(playerGridPosition))
            {
                GameObject.Destroy(visitIndicatorsDictionary[playerGridPosition]);
                visitIndicatorsDictionary.Remove(playerGridPosition);
            }
        }
    }
}