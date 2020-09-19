using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ObjectiveCollection : ScriptableObject
{
    #region Data
    public List<string> objectivePool = new List<string>();
    #endregion

    #region Properties
    public int Count => objectivePool.Count;
    #endregion

#if UNITY_EDITOR
    #region Editor Cache
    public ObjectiveCategory _category;
    public ObjectiveResetCondition _resetCondition;
    public int _count;
    public int _xpReward;
    #endregion

    #region Objective Collection Editor Methods
    public void SaveObjective()
    {
        Objective objective = new Objective(_category, _resetCondition, _count, _xpReward);

        string objectiveJson = JsonUtility.ToJson(objective);

        objectivePool.Add(objectiveJson);

        EditorUtility.SetDirty(this);

        Debug.Log(string.Format("Saved Objective {0}: {1}", objectivePool.Count, 
            ObjectiveAdapter.GetCorrespondingDescription(objective.Category, objective.ResetCondition, objective.Count)));
    }

    public Objective PollObjective()
    {
        if (Count < 1) return null;

        string objectiveJson = objectivePool[Count - 1];

        Objective objective = JsonUtility.FromJson<Objective>(objectiveJson);

        objectivePool.RemoveAt(Count - 1);

        EditorUtility.SetDirty(this);

        return objective;
    }
    
    public void DeleteObjective(int index)
    {
        objectivePool.RemoveAt(index);

        EditorUtility.SetDirty(this);
    }

    public void InsertObjective(int index)
    {
        Objective objective = new Objective(_category, _resetCondition, _count, _xpReward);

        string objectiveJson = JsonUtility.ToJson(objective);

        objectivePool.Insert(index + 1, objectiveJson);

        EditorUtility.SetDirty(this);
    }

    public void ClearAllObjectives()
    {
        objectivePool.Clear();

        EditorUtility.SetDirty(this);
    }
    #endregion

    [MenuItem("Assets/Create/Objective Collection")]
    public static void CreateDefaultLevels()
    {
        string path = EditorUtility.SaveFilePanelInProject("New Objective Collection", "ObjectiveCollection", "Asset", "Save Objective Collection", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(CreateInstance<ObjectiveCollection>(), path);
    }
#endif

    #region Data Accessor Methods
    public Objective PullObjective(int index)
    {
        string objectiveJson = objectivePool[index];

        Objective objective = JsonUtility.FromJson<Objective>(objectiveJson);

        return objective;
    }
    #endregion
}