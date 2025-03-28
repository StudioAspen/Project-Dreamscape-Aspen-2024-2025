using System;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    private GameManager gameManager;
    private WorldManager worldManager;

    [Header("Config")]
    [SerializeField] private int baseEmpowerTokens = 2;
    [SerializeField] private int baseWeakenTokens = 1;

    public int EmpowerTokens { get; private set; }
    public int WeakenTokens { get; private set; }

    [Header("Quests")]
    [SerializeField] private List<ProgressionQuestSO> possibleProgressionQuests = new();
    public const int QUEST_COUNT = 3;
    public ProgressionQuestSO[] CurrentQuests { get; private set; } = new ProgressionQuestSO[QUEST_COUNT];
    public Action<ProgressionQuestSO> OnQuestComplete = delegate { };

    private void Awake()
    {
        worldManager = GetComponent<WorldManager>();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        EmpowerTokens = baseEmpowerTokens;
        WeakenTokens = baseWeakenTokens;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        // If just entering playmode for the first time (ignores pause/unpause and other game state changes)
        if(newState == GameState.PLAYING && gameManager.PreviousState == GameState.EVENT_SELECTION)
        {
            CreateNewQuests();
        }

        // If clearing the event
        if (newState == GameState.ASPECT_SELECTION && gameManager.PreviousState == GameState.PLAYING)
        {
            CleanUpQuests();
        }
    }

    private void Update()
    {
        // Cheat for completing all quests
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.LogWarning("Chear: Insta completing all quests");
            for(int i = 0; i < QUEST_COUNT; i++)
            {
                if (CurrentQuests[i] == null) continue;
                CurrentQuests[i].Complete();
            }
        }

        UpdateQuests();
    }

    /// <summary>
    /// Adds empower tokens
    /// </summary>
    /// <param name="amount">The amount to add</param>
    public void AddEmpowerTokens(int amount)
    {
        // Debug.Log($"Added {amount} empower tokens");
        EmpowerTokens += amount;
    }

    /// <summary>
    /// Adds weaken tokens
    /// </summary>
    /// <param name="amount">The amount to add</param>
    public void AddWeakenTokens(int amount)
    {
        // Debug.Log($"Added {amount} weaken tokens");
        WeakenTokens += amount;
    }

    #region Quests
    /// <summary>
    /// Picks 3 random quests and populates/replaces CurrentQuests array.
    /// Fires when the game state changes to PLAYING from EVENT_SELECTION.
    /// </summary>
    private void CreateNewQuests()
    {
        if(possibleProgressionQuests.Count < QUEST_COUNT)
        {
            Debug.LogWarning($"Progression Manager needs at least {QUEST_COUNT} possible quests before creating any");
            return;
        }

        CleanUpQuests(); // Just in case it wasn't done before

        List<ProgressionQuestSO> remainingQuests = new List<ProgressionQuestSO>(possibleProgressionQuests);

        for(int i = 0; i < QUEST_COUNT; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, remainingQuests.Count);

            ProgressionQuestSO runtimeQuestInstance = Instantiate(remainingQuests[randomIndex]);
            runtimeQuestInstance.Init(this);

            CurrentQuests[i] = runtimeQuestInstance;

            remainingQuests.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// Calls the Update method for the quests if the game is playing.
    /// </summary>
    private void UpdateQuests()
    {
        if (gameManager.CurrentState != GameState.PLAYING) return;

        for(int i = 0; i < QUEST_COUNT; i++)
        {
            if (CurrentQuests[i] == null) continue;

            CurrentQuests[i].Update();
        }
    }

    /// <summary>
    /// Cleans up all quests
    /// </summary>
    private void CleanUpQuests()
    {
        for (int i = 0; i < QUEST_COUNT; i++)
        {
            if (CurrentQuests[i] == null) continue;

            CurrentQuests[i].CleanUp();
            CurrentQuests[i] = null;
        }
    }
    #endregion

    #region During Land Empowerment
    /// <summary>
    /// Continues to the event selection phase, resetting the land level differences and changing the game state.
    /// </summary>
    public void ContinueToEventSelection()
    {
        ResetLandLevelDifferences();

        gameManager.ChangeState(GameState.EVENT_SELECTION);
    }

    /// <summary>
    /// Tries to empower the land at the position of the ghost land.
    /// </summary>
    public void TryEmpowerLandAtGhost()
    {
        Vector2Int spawnPosition = worldManager.GetGridPosition(worldManager.GetGhostLandPosition());

        LandManager hoveredLand = worldManager.GetLandByGridPosition(spawnPosition);

        if (hoveredLand == null)
        {
            //Debug.LogWarning("Can't empower at this land");
            PlayFailedActionSFX();
            return;
        }

        if (hoveredLand.LevelDifference >= 0 && EmpowerTokens <= 0)
        {
            //Debug.LogWarning("Not enough empower tokens");
            PlayFailedActionSFX();
            return;
        }

        if (hoveredLand.LevelDifference < 0) // if already been weakened
        {
            WeakenTokens++;
            EmpowerTokens++;
        }

        if (hoveredLand.TryAddLevel(1))
        {
            PlaySuccessfulEmpowerSFX();
            EmpowerTokens--;
        }
    }

    /// <summary>
    /// Tries to weaken the land at the position of the ghost land.
    /// </summary>
    public void TryWeakenLandAtGhost()
    {
        Vector2Int spawnPosition = worldManager.GetGridPosition(worldManager.GetGhostLandPosition());

        LandManager hoveredLand = worldManager.GetLandByGridPosition(spawnPosition);

        if (hoveredLand == null)
        {
            //Debug.LogWarning("Can't weaken at this land");
            PlayFailedActionSFX();
            return;
        }

        if (hoveredLand.LevelDifference <= 0 && WeakenTokens <= 0)
        {
            //Debug.LogWarning("Not enough weaken tokens");
            PlayFailedActionSFX();
            return;
        }

        if (hoveredLand.LevelDifference > 0) // if already been empowered
        {
            EmpowerTokens++;
            WeakenTokens++;
        }

        if (hoveredLand.TryAddLevel(-1))
        {
            PlaySuccessfulWeakenSFX();
            WeakenTokens--;
        }        
    }

    /// <summary>
    /// Checks if all tokens have been used.
    /// </summary>
    /// <returns>True if all tokens have been used, false otherwise.</returns>
    public bool CanProceedFromEmpowerment()
    {
        return EmpowerTokens + WeakenTokens <= 0;
    }

    /// <summary>
    /// Resets the level differences of all spawned lands.
    /// </summary>
    private void ResetLandLevelDifferences()
    {
        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            land.ResetLevelDifference();
        }
    }

    /// <summary>
    /// Refunds the progression changes made to the lands.
    /// The level difference of each land stores the changes made to the land levels during the progression phase.
    /// </summary>
    public void RefundProgressionChanges()
    {
        foreach (LandManager land in worldManager.SpawnedLands.Values)
        {
            if (land.LevelDifference > 0) EmpowerTokens += Mathf.Abs(land.LevelDifference);
            if (land.LevelDifference < 0) WeakenTokens += Mathf.Abs(land.LevelDifference);

            land.UndoLevelChanges();
        }
    }
    #endregion

    #region SFX

    /// <summary>
    /// Plays the SFX for a failed empower/weaken attempt.
    /// </summary>
    private void PlayFailedActionSFX()
    {
        return; // TODO: when failed action SFX is added, assign here
    }

    /// <summary>
    /// Plays the SFX for a successful land empower.
    /// </summary>
    private void PlaySuccessfulEmpowerSFX()
    {
        AkSoundEngine.PostEvent("EmpowerTokenSelect", gameObject);
    }

    /// <summary>
    /// Plays the SFX for a successful land weaken.
    /// </summary>
    private void PlaySuccessfulWeakenSFX()
    {
        AkSoundEngine.PostEvent("WeakenTokenSelect", gameObject);
    }

    #endregion
}
