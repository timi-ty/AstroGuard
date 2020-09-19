[System.Serializable]
public class Session
{
    #region Singleton
    public static Session Instance { get; set; } = new Session();
    private Session()
    {
        ShipHealth = Ship.FULL_HEALTH;
        StartTime = CurrentTime.MilliSeconds;
    }
    #endregion

    #region Constants
    public const int MAX_SESSION_STRENGTH = 720;
    #endregion

    #region Volatile Data
    [System.NonSerialized]
    private int volatileScore;
    [System.NonSerialized]
    private int restorableScore;
    [System.NonSerialized]
    private int volatileSessionStrength;
    [System.NonSerialized]
    private int restorableSessionStrength;
    [System.NonSerialized]
    private long restorableStartTime;
    [System.NonSerialized]
    private int restorableAstroGold;
    #endregion

    #region Persistent Data
    private int PersistentScore { get; set; }
    private long StartTime { get; set; }
    private int PersistentSessionStrength { get; set; }
    public int ShipHealth { get; set; }
    #endregion

    #region Global Static Accessors
    public static int Score => Instance.volatileScore;
    public static int SessionStrength => Instance.volatileSessionStrength;
    #endregion

    public static void RecordScoreBump()
    {
        Instance.volatileSessionStrength++;

        Instance.volatileSessionStrength = Instance.volatileSessionStrength > MAX_SESSION_STRENGTH ? 
            MAX_SESSION_STRENGTH : Instance.volatileSessionStrength;

        int scoreBump = 1 + Instance.volatileSessionStrength / 30;

        Instance.volatileScore += scoreBump;
    }

    public void Resume()
    {
        volatileScore = PersistentScore;
        volatileSessionStrength = PersistentSessionStrength;
    }

    public void Suspend()
    {
        volatileScore = 0;
        volatileSessionStrength = 0;

        PlayerStats.DiscardPocketedAstroGold();
    }

    public void Bind()
    {
        PersistentScore = volatileScore;
        PersistentSessionStrength = volatileSessionStrength;

        PlayerStats.InsertHighScore(PersistentScore, StartTime);

        PlayerStats.DepositPocketedAstroGold();
    }

    public void Reset()
    {
        Bind();

        restorableScore = volatileScore;

        restorableSessionStrength = volatileSessionStrength;

        restorableStartTime = StartTime;

        restorableAstroGold = PlayerStats.Instance.TempAstroGoldPocket;

        volatileScore = 0;

        volatileSessionStrength = 0;

        PlayerStats.DiscardPocketedAstroGold();

        StartTime = CurrentTime.MilliSeconds;

        Bind();

        Analytics.LogSessionEnded(PersistentScore);
    }

    public void Restore()
    {
        volatileScore = restorableScore;
        volatileSessionStrength = restorableSessionStrength;
        StartTime = restorableStartTime;

        PlayerStats.PocketAstroGold(restorableAstroGold);

        Analytics.LogSessionRestored(volatileScore);
    }


    #region Action Methods
    public void Wipe()
    {
        Instance = new Session();
    }
    #endregion
}