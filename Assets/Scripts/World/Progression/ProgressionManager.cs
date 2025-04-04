using System;
using System.Collections.Generic;
using System.Linq;
using Packages.Rider.Editor.UnitTesting;
using Unity.VisualScripting;
using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    public GameManager gameManager { get; private set; }
    public WorldManager worldManager { get; private set; }
    public EventManager eventManager { get; private set; }

    // Loaded in different scene
    public AspectsManager aspectsManager { get; private set; }
    public Player player { get; private set; }
    public LevelSystem levelSystem { get; private set; }

    [Header("Config")]
    [SerializeField] private int baseEmpowerTokens = 2;
    [SerializeField] private int baseWeakenTokens = 1;

    public int EmpowerTokens { get; private set; }
    public int WeakenTokens { get; private set; }

    // Tutorial Counts as a wave
    public int WaveIndex { get; private set; } = 0;

    [field: Header("Quests")]
    [field: SerializeField] public List<ProgressionQuestSO> PossibleProgressionQuests { get; private set; } = new();
    public List<ProgressionQuestSO> CurrentQuests { get; private set; } = new();
    public Action<ProgressionQuestSO> OnQuestComplete = delegate { };

    // List Storage during runtime
    private List<SkillfulQuestSO> skillfulQuests = new List<SkillfulQuestSO>();
    private List<AspectQuestSO> aspectQuests = new List<AspectQuestSO>();
    private List<WorldEventQuestSO> worldEventQuests = new List<WorldEventQuestSO>();

    private void Awake()
    {
        worldManager = GetComponent<WorldManager>();
        eventManager = GetComponent<EventManager>();
    }

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;

        EmpowerTokens = baseEmpowerTokens;
        WeakenTokens = baseWeakenTokens;

        // Sort the Quests in order of difficulty from least to greatest, just once.
        PossibleProgressionQuests.Sort((a, b) => a.Difficulty.CompareTo(b.Difficulty));

        // Filter the Quests into lists based on their class, just once.
        foreach(ProgressionQuestSO quest in PossibleProgressionQuests)
        {
          switch (quest)
          {
            case SkillfulQuestSO sQ:
              skillfulQuests.Add(sQ);
              break;
            case AspectQuestSO aQ:
              aspectQuests.Add(aQ);
              break;   
            case WorldEventQuestSO eQ:
              worldEventQuests.Add(eQ);
              break;
            default:
              break;
          }
        }

        // Total Time Complexity: n log n
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
            WaveIndex++;
        }

        // If Game Over
        if (newState == GameState.GAME_OVER)
        {
          WaveIndex = 0;
        }
    }

    private void Update()
    {
        // Cheat for completing all quests
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.LogWarning("Cheat: Insta completing all quests");
            for(int i = 0; i < CurrentQuests.Count; i++)
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

        if(PossibleProgressionQuests.Count < 1)
        {
            Debug.LogWarning($"Progression Manager needs at least 1 possible quest before creating any");
            return;
        }

        CleanUpQuests(); // Just in case it wasn't done before

        // After completing the first wave, we'll introduce the first quest type: Skillful Play.
        if (WaveIndex >= 1)
        {
          if (player == null)
            player = FindFirstObjectByType<Player>();
          if (levelSystem == null)
            levelSystem = FindFirstObjectByType<LevelSystem>();

          SkillfulQuestSO skillfulQuestInstance = FindProgressionQuestByType(skillfulQuests);
          if (skillfulQuestInstance != null)
          {
            Instantiate(skillfulQuestInstance);
            skillfulQuestInstance.Init(this);
            CurrentQuests.Add(skillfulQuestInstance);
            skillfulQuests.Remove(skillfulQuestInstance);
          }
        }

        // By the 5th wave, we can assume the player has already unlocked an aspect.
        if (WaveIndex >= 5)
        {
          if (aspectsManager == null)
            aspectsManager = FindFirstObjectByType<AspectsManager>();

          // We'll introduce the second quest type: Aspects.
          if (aspectsManager.EquippedAspectTrees.Length > 0)
          {
            AspectQuestSO aspectQuestInstance = FindProgressionQuestByType(aspectQuests);
            if (aspectQuestInstance != null)
            {
              Instantiate(aspectQuestInstance);
              aspectQuestInstance.Init(this);
              CurrentQuests.Add(aspectQuestInstance);
              aspectQuests.Remove(aspectQuestInstance);
            }
          }
        }

        // By the 8th wave, the player has had enough time to experience each event at least once.
        if (WaveIndex >= 1) 
        {
          if (eventManager == null)
            eventManager = GetComponent<EventManager>();

          // We'll introduce the third quest type: World Events.
          if (eventManager.CurrentEvent != null)
          {
            WorldEventQuestSO worldEventQuestInstance = FindProgressionQuestByType(worldEventQuests);
            if (worldEventQuestInstance != null)
            {
              Instantiate(worldEventQuestInstance);
              worldEventQuestInstance.Init(this);
              CurrentQuests.Add(worldEventQuestInstance);
              worldEventQuests.Remove(worldEventQuestInstance);
            }
          }
        }
    }

    /// <summary>
    /// Calls the Update method for the quests if the game is playing.
    /// </summary>
    private void UpdateQuests()
    {
        if (gameManager.CurrentState != GameState.PLAYING) return;

        foreach (ProgressionQuestSO quest in CurrentQuests)
          quest.Update();
    }

    /// <summary>
    /// Cleans up all quests
    /// </summary>
    private void CleanUpQuests()
    {
        // for (int i = 0; i < CurrentQuests.Count; i++)
        // {
        //     if (CurrentQuests[i] == null) continue;

        //     CurrentQuests[i].CleanUp();
        //     CurrentQuests[i] = null;
        // }
        if (CurrentQuests.Count == 0)
          return;

        foreach (ProgressionQuestSO quest in CurrentQuests)
          quest.CleanUp();

        CurrentQuests.Clear();
    }

  private T FindProgressionQuestByType<T>(List<T> quests) where T : ProgressionQuestSO
  {
    if (quests.Count == 0)
      return null;
    else if (quests.Count == 1 && quests[0].MeetsCriteria(this))
      return quests[0]; 
    
    // Start with lowest difficulty
    int difficultyIndex = 0;
    
    // Try different difficulty levels if needed
    while (difficultyIndex < quests.Count)
    {
      int currentDifficulty = quests[difficultyIndex].Difficulty;
      
      // Find how many quests have this difficulty
      int sameQuestDifficultyCount = 0;
      while (difficultyIndex + sameQuestDifficultyCount < quests.Count && 
            quests[difficultyIndex + sameQuestDifficultyCount].Difficulty == currentDifficulty)
      {
        sameQuestDifficultyCount++;
      }
      
      // Try a reasonable number of attempts at this difficulty
      int maxAttempts = Math.Min(sameQuestDifficultyCount * 2, 20);
      for (int a = 0; a < maxAttempts; a++)
      {
        // Pick a random quest of the current difficulty
        int randomOffset = UnityEngine.Random.Range(0, sameQuestDifficultyCount);
        T quest = quests[difficultyIndex + randomOffset];
        
        // If the player can complete it, we return it
        if (quest.MeetsCriteria(this))
          return quest;
      }
      
      // Move to the next difficulty level
      difficultyIndex += sameQuestDifficultyCount;
    }
    
    return null;
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
