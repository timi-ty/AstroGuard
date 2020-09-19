﻿//In Progress
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectivesState
{
    #region Singleton
    public static ObjectivesState Instance { get; set; } = new ObjectivesState();
    private ObjectivesState() 
    {
        LastPulledObjetiveIndex = 0;
        CompletedObjectives = new List<Objective>();
        ActiveObjectives = new Objective[3];
    }
    #endregion

    public int LastPulledObjetiveIndex { get; set; }
    public List<Objective> CompletedObjectives { get; set; }
    public Objective[] ActiveObjectives { get; set; }

    #region Action Methods
    public void Wipe()
    {
        Instance = new ObjectivesState();
    }
    #endregion
}

public class ObjectiveManager : MonoBehaviour
{
    #region Singleton
    public static ObjectiveManager instance { get; private set; }
    #endregion

    #region Data Sources
    [Header("Data Sources")]
    public ObjectiveCollection objectiveCollection;
    #endregion

    #region Objective State Pointers
    private Objective[] activeObjectives 
    { 
        get 
        { 
            return ObjectivesState.Instance.ActiveObjectives; 
        } 
        set 
        { 
            ObjectivesState.Instance.ActiveObjectives = value; 
        } 
    }
    private List<Objective> completedObjectives 
    {
        get
        {
            return ObjectivesState.Instance.CompletedObjectives;
        }
        set
        {
            ObjectivesState.Instance.CompletedObjectives = value;
        } 
    }
    public int lastPulledObjetiveIndex 
    {
        get
        {
            return ObjectivesState.Instance.LastPulledObjetiveIndex;
}
        set
        {
            ObjectivesState.Instance.LastPulledObjetiveIndex = value;
        } 
    }
    #endregion


    private void Awake()
    {
        #region Singleton
        if (!instance)
        {
            instance = this;
        }
        else if (!instance.Equals(this))
        {
            Destroy(gameObject);
        }
        #endregion
    }


    public static void Refresh()
    {
        if (!GameManager.IsPlayerAlive()) return;

        instance.UpdateObjectives();
    } 

    private void UpdateObjectives()
    {
        for(int i = 0; i < 3; i++)
        {
            Objective objective = activeObjectives[i];

            if (IsEmpty(objective) && MoreObjectivesAvailable())
            {
                objective = GetNewObjective();
            }
            else if(!IsEmpty(objective))
            {
                bool previouslyCompleted = objective.IsCompleted;

                objective.UpdateCompletion();

                if (IsComplete(objective))
                {
                    if (!previouslyCompleted)
                    {
                        completedObjectives.Add(objective);

                        UIManager.Notify(objective);

                        PlayerStats.ObjectiveReward(objective.XpReward, objective.GoldReward);

                        FirebaseUtility.RecordCustomEvent("Objective Completed",
                            new Firebase.Analytics.Parameter[]
                            {
                                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterLevel, GameManager.currentLevel),
                                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterCharacter, PlayerStats.Instance.ExperienceLevel),
                                new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterItemName, objective.Description)
                            });
                    }

                    if (MoreObjectivesAvailable())
                    {
                        objective = GetNewObjective();
                    }
                }
            }

            activeObjectives[i] = objective;
        }
    }

    public static Objective GetActiveObjective(int i)
    {
        return instance.activeObjectives[i];
    }

    #region Utility Methods
    private Objective GetNewObjective()
    {
        Objective objective = objectiveCollection.PullObjective(lastPulledObjetiveIndex);

        lastPulledObjetiveIndex++;

        objective.Activate();

        return objective;
    }

    private bool IsEmpty(Objective objective)
    {
        return objective == null;
    }

    private bool IsComplete(Objective objective)
    {
        return objective.IsCompleted;
    }

    private bool MoreObjectivesAvailable()
    {
        return lastPulledObjetiveIndex < objectiveCollection.Count - 1;
    }
    #endregion
}