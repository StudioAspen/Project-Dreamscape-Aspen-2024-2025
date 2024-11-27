using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ComboDataSO))]
public class ComboDataSOEditor : Editor
{
    ///-/////////////////////////////////////////////////////////////////////////////////////
    ///
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ComboDataSO comboData = (ComboDataSO)target;
        if (comboData.IsComboClipValid == false)
        {
            EditorGUILayout.HelpBox("The selected combo clip is not available in the PlayerAnimationController", MessageType.Error);
        }
    }
}

#endif // UNITY_EDITOR
