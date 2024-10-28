#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class PivotAlignmentWindow : EditorWindow
{
    private static PivotAlignmentWindow win;
    private GameObject gameObject;
    private MeshRenderer renderer;
    private string[] options = new string[] { "Original", "Center", "Min", "Max" };
    private enum optionsIndex { Original, Center, Min, Max };
    private optionsIndex selectedAlignmentX;
    private optionsIndex selectedAlignmentY;
    private optionsIndex selectedAlignmentZ;
    private float xOffset;
    private float yOffset;
    private float zOffset;

    [MenuItem("Dreamscape/PivotAlignment")]
    public static void InitPivotAlignmentTool()
    {
        InitWindow();
    }

    public static void InitWindow()
    {
        win = EditorWindow.GetWindow<PivotAlignmentWindow>("Pivot Alignment");
        win.Show();
        // Subscribe to selection changed event
        Selection.selectionChanged += win.OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        Repaint();
    }

    private void OnGUI()
    {
        // Instantiate GameObject and Renderer and print errors
        gameObject = Selection.activeGameObject;
        if (gameObject == null)
        {
            EditorGUILayout.LabelField("No GameObject is selected");
            return;
        }
        if (gameObject.transform.parent == null)
        {
            EditorGUILayout.LabelField("Selected GameObject has no parent");
            return;
        }
        renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            EditorGUILayout.LabelField("Selected GameObject has no mesh renderer");
            return;
        }

        // DropDown Menus
        EditorGUILayout.LabelField("Alignment Options", EditorStyles.boldLabel);
        selectedAlignmentX = (optionsIndex)EditorGUILayout.Popup(new GUIContent("X"), (int)selectedAlignmentX, options);
        xOffset = EditorGUILayout.FloatField("X Offset:", xOffset);
        selectedAlignmentY = (optionsIndex)EditorGUILayout.Popup(new GUIContent("Y"), (int)selectedAlignmentY, options);
        yOffset = EditorGUILayout.FloatField("Y Offset:", yOffset);
        selectedAlignmentZ = (optionsIndex)EditorGUILayout.Popup(new GUIContent("Z"), (int)selectedAlignmentZ, options);
        zOffset = EditorGUILayout.FloatField("Z Offset:", zOffset);

        // Move GameObject Button
        if (GUILayout.Button("Apply Alignment", GUILayout.Height(35), GUILayout.ExpandWidth(true)))
        {
            gameObject.transform.position = gameObject.transform.parent.position;
            float _xPos = DropDownSelection(selectedAlignmentX).x;
            float _yPos = DropDownSelection(selectedAlignmentY).y;
            float _zPos = DropDownSelection(selectedAlignmentZ).z;
            gameObject.transform.position = new Vector3(_xPos + xOffset, _yPos + yOffset, _zPos + zOffset);
        }
    }

    private Vector3 DropDownSelection(optionsIndex _selectedAlignment)
    {
        switch (_selectedAlignment)
        {
            case optionsIndex.Original:
                return gameObject.transform.parent.position;
            case optionsIndex.Center:
                return 2 * gameObject.transform.parent.position - renderer.bounds.center;
            case optionsIndex.Min:
                return 2 * gameObject.transform.parent.position - renderer.bounds.center + renderer.bounds.size / 2;
            case optionsIndex.Max:
                return 2 * gameObject.transform.parent.position - renderer.bounds.center - renderer.bounds.size / 2;
            default:
                return gameObject.transform.parent.position;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the selection changed event when the window is closed
        Selection.selectionChanged -= OnSelectionChanged;
    }
}
#endif // UNITY_EDITOR
