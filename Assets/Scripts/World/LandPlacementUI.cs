using UnityEngine;

public class LandPlacementUI : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState newState)
    {
        if (newState != GameState.LAND_PLACEMENT)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }
}