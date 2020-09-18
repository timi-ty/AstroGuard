//In Progress
using System;

[System.Serializable]
public class GameData
{
    #region Singleton
    private static GameData _instance = new GameData();
    public static GameData Instance 
    {
        get => _instance;
        set
        {
            _instance = value;

            LevelsState.Instance = _instance.LevelsState;
            ObjectivesState.Instance = _instance.ObjectivesState;
            Metrics.Instance = _instance.Metrics;
            Session.Instance = _instance.Session;
            PlayerStats.Instance = _instance.PlayerStats;
        }
    }

    private GameData()
    {
        LevelsState = LevelsState.Instance;
        ObjectivesState = ObjectivesState.Instance;
        Metrics = Metrics.Instance;
        Session = Session.Instance;
        PlayerStats = PlayerStats.Instance;

        DeviceID = ApplicationManager.DeviceID;
    }
    #endregion

    #region Data
    private LevelsState LevelsState { get; set; }
    private ObjectivesState ObjectivesState { get; set; }
    private Metrics Metrics { get; set; }
    private Session Session { get; set; }
    private PlayerStats PlayerStats { get; set; }
    #endregion

    #region Metadata
    public string DeviceID { get; set; }
    public string UserID { get; private set; }
    public DateTime TimeStamp { get; private set; }
    #endregion

    #region Structure
    /// <summary>
    /// Transfers ownership of the GameData object to the current User and the current device.
    /// </summary>
    public void Sign()
    {
        TimeStamp = DateTime.Now;
        DeviceID = ApplicationManager.DeviceID;

        if (UserID == null) UserID = FirebaseUtility.CurrentUser?.UserId;
    }

    /// <summary>
    /// Wipes the GameData Instance clean.
    /// </summary>
    public void Reset()
    {
        Clear();

        UserID = null;
    }
    #endregion

    #region Operations
    /// <summary>
    /// Clears all progress.
    /// </summary>
    public void Clear(){
        ClearLevelProgress();
        ClearObjectiveCompletion();
        ClearMetrics();
        ClearSession();
        ClearPlayerStats();

        Instance = new GameData();
    }

    public void ClearLevelProgress()
    {
        LevelsState.Wipe();
    }

    public void ClearObjectiveCompletion()
    {
        ObjectivesState.Wipe();
    }

    public void ClearMetrics()
    {
        Metrics.Wipe();
    }

    public void ClearSession()
    {
        Session.Wipe();
    }

    public void ClearPlayerStats()
    {
        PlayerStats.Wipe();
    }
    #endregion
}
