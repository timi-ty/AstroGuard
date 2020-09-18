//In Progress
using UnityEngine;

[System.Serializable]
public class Objective
{
    #region Unity Serialize Fields
    [SerializeField]
    private ObjectiveCategory category;
    [SerializeField]
    private ObjectiveResetCondition resetCondition;
    [SerializeField]
    private int count;
    [SerializeField]
    private int xpReward;
    #endregion

    #region Properties
    public ObjectiveCategory Category { get => category; private set => category = value; }
    public ObjectiveResetCondition ResetCondition { get => resetCondition; private set => resetCondition = value; }
    public int Count { get => count; private set => count = value; }
    public int XpReward { get => xpReward; private set => xpReward = value; }
    public int GoldReward { get => xpReward * 25; }
    public string Description { get; private set; }
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Completion of the objective in percentage.
    /// </summary>
    public float Completion { get; private set; }
    #endregion

    #region Snapshots
    private int startSnapshot_Main;
    private int startSnapshot_Condition;
    #endregion

    public Objective(ObjectiveCategory category, ObjectiveResetCondition resetCondition, int count, int xpReward)
    {
        Category = category;
        ResetCondition = resetCondition;
        Count = count;
        XpReward = xpReward;
    }

    /// <summary>
    /// Initializes and activates the objective. Should be called ONLY ONCE when the objective becomes active.
    /// </summary>
    public void Activate()
    {
        startSnapshot_Main = ObjectiveAdapter.GetCorrespondingMetric(Category);
        startSnapshot_Condition = ObjectiveAdapter.GetCorrespondingMetric(ResetCondition);
        Description = ObjectiveAdapter.GetCorrespondingDescription(Category, ResetCondition, Count);

        Completion = 0;
    }

    /// <summary>
    /// Updates the objective. Can be called any number of times anywhere that is appropriate.
    /// </summary>
    public void UpdateCompletion()
    {
        int currentSnapshot_Main = ObjectiveAdapter.GetCorrespondingMetric(Category);
        int currentSnapshot_Condition = ObjectiveAdapter.GetCorrespondingMetric(ResetCondition);

        int progress = currentSnapshot_Main - startSnapshot_Main;

        bool withinCondition = startSnapshot_Condition == currentSnapshot_Condition;

        if (withinCondition && progress >= 0)
        {
            Completion = 100 * ((float) progress) / Count;

            if (progress >= Count)
            {
                MarkComplete();
            }
        }
        else
        {
            Reset();
        }
    }

    private void Reset()
    {
        Activate();
    }

    private void MarkComplete()
    {
        Completion = 100;
        IsCompleted = true;
    }
}
