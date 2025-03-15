using UnityEngine;

public abstract class ProgressionQuestSO : ScriptableObject
{
    private protected ProgressionManager progressionManager;

    public enum Reward
    {
        EMPOWER_TOKEN,
        WEAKEN_TOKEN
    }

    [field: SerializeField] public string ObjectiveText { get; private set; } = "";
    [field: SerializeField] public Reward CompletionReward { get; private set; }
    public bool IsCompleted { get; protected set; }

    /// <summary>
    /// Initializes instance of quest and calls the OnActivated() method.
    /// </summary>
    /// <param name="progressionManager">Reference to the progression manager</param>
    public void Init(ProgressionManager progressionManager)
    {
        this.progressionManager = progressionManager;

        //Debug.Log($"Activated progression quest: {name}");
        OnActivated();
    }

    /// <summary>
    /// Fires once when the game enters the PLAYING state. The player auto accepts progression quests every new event.
    /// </summary>
    private protected abstract void OnActivated();

    /// <summary>
    /// Call this function to complete the quest and trigger the CleanUp() cleanup method.
    /// Awards the player with the reward token.
    /// </summary>
    public void Complete()
    {
        //Debug.Log($"Completed progression quest: {name}");

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

        CleanUp();
    }

    /// <summary>
    /// Can be called if you want to avoid clean up 
    /// </summary>
    private protected void CompleteWithoutCleanUp()
    {
        IsCompleted = true;

        if (CompletionReward == Reward.EMPOWER_TOKEN)
        {
            progressionManager.AddEmpowerTokens(1);
        }
        else
        {
            progressionManager.AddWeakenTokens(1);
        }

        progressionManager.OnQuestComplete.Invoke(this);
    }

    /// <summary>
    /// Cleans up the quest. Fired when quest is completed or by the progression manager when clearing the event.
    /// </summary>
    public void CleanUp()
    {
        //Debug.Log($"Cleaned Up progression quest: {name}");
        OnCleanUp();
    }

    /// <summary>
    /// Fires once when the quest cleaned up.
    /// </summary>
    private protected abstract void OnCleanUp();

    /// <summary>
    /// Called by the progression manager's Update method.
    /// </summary>
    public void Update()
    {
        if(IsCompleted) return;

        OnUpdate();
    }

    /// <summary>
    /// Updates every frame while the quest is not completed.
    /// </summary>
    private protected abstract void OnUpdate();
}
