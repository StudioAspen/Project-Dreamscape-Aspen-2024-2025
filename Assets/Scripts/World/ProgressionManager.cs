using UnityEngine;

public class ProgressionManager : MonoBehaviour
{
    private GameManager gameManager;
    private WorldManager worldManager;

    public int EmpowerTokens { get; private set; }
    public int WeakenTokens { get; private set; }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        worldManager = GetComponent<WorldManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.LAND_EMPOWERMENT)
        {
            RestockTokens();
        }
    }

    /// <summary>
    /// Continues to the event selection phase, resetting the land level differences and changing the game state.
    /// </summary>
    public void ContinueToEventSelection()
    {
        ResetLandLevelDifferences();

        gameManager.ChangeState(GameState.EVENT_SELECTION);
    }

    /// <summary>
    /// Restocks the empower and weaken tokens based on the number of lands.
    /// </summary>
    private void RestockTokens()
    {
        EmpowerTokens = Mathf.CeilToInt((worldManager.SpawnedLands.Count - 1) / 2f);
        WeakenTokens = Mathf.FloorToInt((worldManager.SpawnedLands.Count - 1) / 2f);
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
            return;
        }

        if (hoveredLand.LevelDifference >= 0 && EmpowerTokens <= 0)
        {
            //Debug.LogWarning("Not enough empower tokens");
            return;
        }

        if (hoveredLand.LevelDifference < 0) // if already been weakened
        {
            WeakenTokens++;
            EmpowerTokens++;
        }

        hoveredLand.AddLevel(1);
        EmpowerTokens--;
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
            return;
        }

        if (hoveredLand.LevelDifference <= 0 && WeakenTokens <= 0)
        {
            //Debug.LogWarning("Not enough weaken tokens");
            return;
        }

        if (hoveredLand.LevelDifference > 0) // if already been empowered
        {
            EmpowerTokens++;
            WeakenTokens++;
        }

        hoveredLand.AddLevel(-1);
        WeakenTokens--;
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
}
