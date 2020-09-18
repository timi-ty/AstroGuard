using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Background))]
public class BackgroundEditor : Editor
{
    private Background Background { get; set; }
    private int backGroundIndex { get; set; }

    private void OnEnable()
    {
        Background = (Background) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(15);

        EditorGUILayout.LabelField("Operate");

        backGroundIndex = EditorGUILayout.IntField("Background Index", backGroundIndex);

        if(GUILayout.Button("Set Background With Index"))
        {
            Background.SetBackground(backGroundIndex);
            EditorUtility.SetDirty(Background);
        }
    }
}
#endif