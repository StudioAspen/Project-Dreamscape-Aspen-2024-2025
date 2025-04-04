using UnityEditor.EditorTools;
using UnityEngine;

public abstract class ProgressionQuestSO : ScriptableObject
{
    private protected ProgressionManager progressionManager;

    public enum Reward
    {
        WEAKEN_TOKEN,
        EMPOWER_TOKEN,
        // below are other potential rewards 

        // HEALTH_RECOVERY,
        // EXP_POINTS,
        // RAGE_UPGRADE,
        // FEAR_UPGRADE
    }

    [field: Header("Basic Settings")]
    [field: SerializeField] public string ObjectiveText { get; protected set; } = "";
    [field: SerializeField] public Reward CompletionReward { get; private set; }
    [field: SerializeField] public bool LogErrorMessages { get; private set; } = false;

    /// <summary>
    /// Did the player complete this quest?
    /// </summary>
    /// <value></value>
    public bool IsCompleted { get; protected set; }

    [Header("Basic Criteria")]
    /// <summary>
    /// The difficulty level of the quest on a scale from 1-3. The Progression Manager prioritizes lower difficulty quests when selecting them.
    /// </summary>
    [Tooltip("The difficulty level of the quest on a scale from 1-3. The Progression Manager prioritizes lower difficulty quests when selecting them.")]
    [Range(1, 3)]
    [SerializeField] protected int difficulty;
    public int Difficulty => difficulty;

    /// <summary>
    /// Initializes instance of quest and calls the OnActivated() method.
    /// </summary>
    /// <param name="progressionManager">Reference to the Progression Manager</param>
    public void Init(ProgressionManager progressionManager)
    {
        this.progressionManager = progressionManager;

        Debug.Log($"Activated progression quest: {name}");
        OnActivated();
    }

    /// <summary>
    /// Checks if the quest meets the minimum criteria before the Progression Manager assigns it.
    /// </summary>
    /// <param name="progressionManager">Reference to the Progression Manager</param>
    /// <returns>A boolean</returns>
    public abstract bool MeetsCriteria(ProgressionManager progressionManager);

    /// <summary>
    /// Fires once when the game enters the PLAYING state. The player auto accepts progression quests every new event.
    /// </summary>
    private protected abstract void OnActivated();

    /// <summary>
    /// Call this function to complete the quest and trigger the CleanUp() cleanup method.
    /// Awards the player with the reward token.
    /// </summary>
    public void Complete(bool withCleanUp = true)
    {
        Debug.Log($"Completed progression quest: {name}");

        IsCompleted = true;

        if(CompletionReward == Reward.EMPOWER_TOKEN)
        {
            progressionManager.AddEmpowerTokens(1);
        }
        else
        {
            progressionManager.AddWeakenTokens(1);
        }

        progressionManager.OnQuestComplete.Invoke(this);

        if(withCleanUp)
          CleanUp();
    }

    /// <summary>
    /// Cleans up the quest. Fired when quest is completed or by the Progression Manager when clearing the event.
    /// </summary>
    public void CleanUp()
    {
        Debug.Log($"Cleaned Up progression quest: {name}");
        OnCleanUp();
    }

    /// <summary>
    /// Fires once when the quest cleaned up.
    /// </summary>
    private protected abstract void OnCleanUp();

    /// <summary>
    /// Called by the Progression Manager's Update method.
    /// </summary>
    public void Update()
    {
      if(IsCompleted) 
        return;

      OnUpdate();
    }

    /// <summary>
    /// Updates every frame while the quest is not completed.
    /// </summary>
    private protected abstract void OnUpdate();
}
