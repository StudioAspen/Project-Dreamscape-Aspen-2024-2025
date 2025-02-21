using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
using UPlayable.AnimationMixer;

[CanEditMultipleObjects]
[CustomEditor(typeof(BaseAnimationOutput), true)]
public class BaseAnimationOutputEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Play", GUILayout.Height(30)))
        {
            ((BaseAnimationOutput)target).Play();
        }
    }
}
#endif