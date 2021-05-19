using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelShrinker))]
public class LevelShrinkerEditor : Editor
{
    #region Target
    private LevelShrinker levelShrinker { get; set; }
    #endregion

    #region GUI Styles
    GUIStyle bold;
    #endregion

    #region GUI Layout Options
    GUILayoutOption[] minorButtonLayout;
    GUILayoutOption[] majorButtonLayout;
    #endregion

    private GUIStyle BoldStyle()
    {
        GUIStyle bold = new GUIStyle();
        bold.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        bold.fontStyle = FontStyle.Bold;
        return bold;
    }

    private void OnEnable()
    {
        levelShrinker = (LevelShrinker)target;

        bold = BoldStyle();

        Debug.Log("Shrinking Levels With: " + levelShrinker.name);

        majorButtonLayout = new GUILayoutOption[] { GUILayout.MinHeight(30), GUILayout.MaxHeight(40), GUILayout.ExpandHeight(true) };
        minorButtonLayout = new GUILayoutOption[] { GUILayout.MinHeight(20), GUILayout.MaxHeight(30), GUILayout.ExpandHeight(true) };
    }

    public override void OnInspectorGUI()
    {
        levelShrinker.levelCollection = (LevelCollection)EditorGUILayout.
            ObjectField("Level Collection", levelShrinker.levelCollection, typeof(LevelCollection), false);

        if (!levelShrinker.levelCollection) return;

        EditorGUILayout.Space();

        if (GUILayout.Button(new GUIContent("Parse Levels", "Read all available levels from level collection."), minorButtonLayout))
        {
            levelShrinker.ParseLevels();
        }

        EditorGUILayout.LabelField(levelShrinker.ShrunkLevelsCount + " Levels", bold);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Shrink Levels", bold);

        levelShrinker.startLevel = EditorGUILayout.IntSlider("Start Level", levelShrinker.startLevel, 1, levelShrinker.ShrunkLevelsCount);
        levelShrinker.endLevel = EditorGUILayout.IntSlider("End Level", levelShrinker.endLevel, 1, levelShrinker.ShrunkLevelsCount);

        levelShrinker.startLevel = Mathf.Min(levelShrinker.startLevel, levelShrinker.endLevel);
        levelShrinker.endLevel = Mathf.Max(levelShrinker.startLevel, levelShrinker.endLevel);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.MinMaxSlider("To Duration Range", ref levelShrinker.minLevelDurVal, ref levelShrinker.maxLevelDurVal, 
            LevelShrinker.MIN_LEVEL_DUR_LIMIT, LevelShrinker.MAX_LEVEL_DUR_LIMIT);
        levelShrinker.minLevelDurVal = EditorGUILayout.FloatField(levelShrinker.minLevelDurVal);
        levelShrinker.maxLevelDurVal = EditorGUILayout.FloatField(levelShrinker.maxLevelDurVal);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space();
        EditorGUILayout.Space();


        if (GUILayout.Button(new GUIContent("Shrink Levels", "Truncate selected levels to duration range specified."), minorButtonLayout))
        {
            levelShrinker.ShrinkLevels();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button(new GUIContent("Publish Shrunk Levels", "Overwrite level collection with shrunk levels."), majorButtonLayout))
        {
            levelShrinker.PublishShrunkLevels();
        }
    }
}
#endif