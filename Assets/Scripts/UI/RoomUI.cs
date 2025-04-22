using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    private InputManager inputManager;

    [Header("Scenes")]
    [SerializeField] private SceneReference loadingScene;

    [Header("UI")]
    [SerializeField] private Button startButton;

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();

        inputManager.OnControlSchemeChanged += InputManager_OnControlSchemeChanged;

        startButton.onClick.AddListener(StartButton_OnClicked);
    }

    private void OnDestroy()
    {
        inputManager.OnControlSchemeChanged -= InputManager_OnControlSchemeChanged;

        startButton.onClick.RemoveListener(StartButton_OnClicked);
    }

    private void InputManager_OnControlSchemeChanged(InputManager.ControlScheme newControlScheme)
    {
        if (newControlScheme == InputManager.ControlScheme.GAMEPAD)
        {
            inputManager.LockCursor();
            // Set the play button as selected
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }
        else
        {
            inputManager.UnlockCursor();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void StartButton_OnClicked()
    {
        SceneManager.LoadScene(loadingScene.Name);
    }
}
