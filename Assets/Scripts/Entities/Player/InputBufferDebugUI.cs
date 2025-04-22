using UnityEngine;

public class InputBufferDebugUI : MonoBehaviour
{
    private PlayerInputReader playerInputReader;

    [SerializeField] private int fontSize = 36; // Added for scaling text
    [SerializeField] private float scaleFactor = 3f; // Scale multiplier for area

    private void Start()
    {
        playerInputReader = FindObjectOfType<PlayerInputReader>();
    }

    private void OnGUI()
    {
        // Scale the GUI area and font
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { fontSize = fontSize };

        // Set area position in bottom-left corner
        float scaledWidth = 200 * scaleFactor;
        float scaledHeight = 200 * scaleFactor;
        float xPos = 10; // 10px padding from the left
        float yPos = Screen.height - scaledHeight - 10; // 10px padding from the bottom

        GUILayout.BeginArea(new Rect(xPos, yPos, scaledWidth, scaledHeight));
        GUILayout.Label("Input Buffer", labelStyle);

        foreach (var (action, timestamp) in playerInputReader.InputBuffer)
        {
            GUILayout.Label($"{action} - {timestamp}", labelStyle);
        }

        GUILayout.EndArea();
    }
}