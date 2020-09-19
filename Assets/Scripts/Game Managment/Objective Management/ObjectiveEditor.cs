using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectiveCollection))]
public class ObjectiveEditor : Editor
{
    #region Target
    private ObjectiveCollection objectiveCollection { get; set; }
    #endregion

    #region Options
    private bool isObjectivesListed { get; set; }
    #endregion

    #region GUI Styles
    GUIStyle bold;
    #endregion

    private void OnEnable()
    {
        bold = BoldStyle();

        objectiveCollection = (ObjectiveCollection) target;
    }

    public override void OnInspectorGUI()
    {
        ObjectiveAdder();

        EditorGUILayout.Space(25);

        ListObjectives();
    }

    private GUIStyle BoldStyle()
    {
        GUIStyle bold = new GUIStyle();
        bold.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        bold.fontStyle = FontStyle.Bold;
        return bold;
    }

    private void ObjectiveAdder()
    {
        EditorGUILayout.LabelField("Add Objective", bold);

        EditorGUILayout.Space();

        objectiveCollection._category = (ObjectiveCategory) EditorGUILayout.EnumPopup("Category", objectiveCollection._category);

        objectiveCollection._resetCondition = (ObjectiveResetCondition) EditorGUILayout.EnumPopup("Reset Condition", objectiveCollection._resetCondition);

        objectiveCollection._count = EditorGUILayout.IntField("Count", objectiveCollection._count);

        objectiveCollection._xpReward = EditorGUILayout.IntField("XP Reward", objectiveCollection._xpReward);

        EditorGUILayout.Space();

        if (GUILayout.Button("Add"))
        {
            objectiveCollection.SaveObjective();
        }

        if (GUILayout.Button("Poll"))
        {
            Objective objective = objectiveCollection.PollObjective();

            objectiveCollection._category = objective.Category;

            objectiveCollection._resetCondition = objective.ResetCondition;

            objectiveCollection._count = objective.Count;

            objectiveCollection._xpReward = objective.XpReward;
        }
    }

    private void ListObjectives()
    {
        EditorGUILayout.LabelField("Objectives:  " + objectiveCollection.Count, bold);

        EditorGUILayout.Space();

        if (isObjectivesListed)
        {
            if (GUILayout.Button("Unlist Objectives"))
            {
                isObjectivesListed = false;
            }

            for (int i = 0; i < objectiveCollection.Count; i++)
            {
                Objective objective = objectiveCollection.PullObjective(i);

                EditorGUILayout.EnumPopup("Category", objective.Category);

                EditorGUILayout.EnumPopup("Reset Condition", objective.ResetCondition);

                EditorGUILayout.IntField("Count", objective.Count);

                EditorGUILayout.IntField("XP Reward", objective.XpReward);

                if (GUILayout.Button("Insert"))
                {
                    objectiveCollection.InsertObjective(i);
                }

                if (GUILayout.Button("Delete"))
                {
                    objectiveCollection.DeleteObjective(i);
                }

                EditorGUILayout.Space();
            }
        }
        else if (GUILayout.Button("List Objectives"))
        {
            isObjectivesListed = true;
        }

        EditorGUILayout.Space(100);

        if (GUILayout.Button("Clear"))
        {
            objectiveCollection.ClearAllObjectives();
        }
    }
}
#endif