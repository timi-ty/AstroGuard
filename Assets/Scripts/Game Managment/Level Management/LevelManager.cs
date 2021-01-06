//In Progress
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class LevelsState
{
    #region Singleton
    public static LevelsState Instance { get; set; } = new LevelsState();
    private LevelsState() 
    {
        LastLevelCompleted = 0;
        Completed = new Dictionary<int, bool>();
        Unlocked = new Dictionary<int, bool>();
        CachedDLCLevels = new List<LevelInfo>();

        Unlocked[1] = true;
    }
    #endregion

    #region Data
    public int LastLevelCompleted { get; set; }
    private Dictionary<int, bool> Completed { get; set; }
    private Dictionary<int, bool> Unlocked { get; set; }
    private List<LevelInfo> CachedDLCLevels { get; set; }
    #endregion

    #region Static Global Accessors
    public static bool IsCompleted(int level)
    {
        Instance.Completed.TryGetValue(level, out bool isCompleted);
        return isCompleted;
    }

    public static bool IsUnlocked(int level)
    {
        Instance.Unlocked.TryGetValue(level, out bool isUnlocked);
        return isUnlocked;
    }

    public static List<LevelInfo> GetCachedDLCLevels()
    {
        return Instance.CachedDLCLevels;
    }
    #endregion

    #region Action Methods
    public void RewriteDLCLevelCache(List<LevelInfo> dLCLevels)
    {
        Instance.CachedDLCLevels = dLCLevels;
    }

    public static void MarkCompleted(int level)
    {
        Instance.Completed[level] = true;
    }

    public static void MarkUnlocked(int level)
    {
        Instance.Unlocked[level] = true;
    }

    public void Wipe()
    {
        Instance = new LevelsState();
    } 
    #endregion
}

public class LevelManager : MonoBehaviour
{
    #region Singleton
    public static LevelManager instance { get; private set; }
    #endregion

    #region Constants
    public const string DLC_LEVELS_PATH = ApplicationManager.DB_DLC_PATH + "Levels";
    public const int TUTORIAL_LEVEL = -1;
    #endregion

    #region Global Static Parameters
    public static bool IsCurrentLevelInfinite => CurrentLevel == int.MaxValue;
    public static int CurrentLevel { get; private set; }
    public static int LevelCount => instance.liveLevels.Count;
    #endregion

    #region Data Sources
    public LevelCollection defaultLevels;
    #endregion

    #region Live Data
    private List<LevelInfo> liveLevels = new List<LevelInfo>();
    #endregion

    #region Pointers
    private static int LastLevelCompleted { get => LevelsState.Instance.LastLevelCompleted; set => LevelsState.Instance.LastLevelCompleted = value; }
    #endregion

    #region Events
    private static event Action LevelsLoaded;
    #endregion

    #region Unity Runtime
    void Awake()
    {
        #region Singleton
        if (!instance)
        {
            instance = this;

            Load();
        }
        else if (!instance.Equals(this))
        {
            Destroy(gameObject);
        }
        #endregion
    }
    #endregion

    #region Loading Methods
    private void Load()
    {
        LoadDefaultLevels();

        //LoadCachedDLCLevels(); //Disabled remote levels

        //FirebaseUtility.AddDatabaseReadyListener(LoadNewDLCLevels);
    }

    private void LoadDefaultLevels()
    {
        liveLevels = new List<LevelInfo>();

        liveLevels.AddRange(defaultLevels.GetAll());

        LevelsLoaded?.Invoke();
    }

    private void LoadCachedDLCLevels()
    {
        List<LevelInfo> dlcLevels = LevelsState.GetCachedDLCLevels();

        Debug.Log("Found " + dlcLevels.Count.ToString() + " cached DLC Levels.");

        LoadDefaultLevels();

        liveLevels.AddRange(dlcLevels);

        LevelsLoaded?.Invoke();
    }

    private void LoadNewDLCLevels()
    {
        List<LevelInfo> dlcLevels = new List<LevelInfo>();

        int defaultLevelsCount = defaultLevels.Count;

        FirebaseUtility.QueryFromDatabase(DLC_LEVELS_PATH,
            (dlcLevelsJsons) =>
            {
                int dlcLevelsCount = dlcLevelsJsons.Count;

                Debug.Log("Found " + dlcLevelsJsons.Count.ToString() + " new DLC Levels.");

                for (int i = 0; i < dlcLevelsCount; i++)
                {
                    LevelInfo dlcLevelInfo = JsonUtility.FromJson<LevelInfo>((string)dlcLevelsJsons[i]);

                    dlcLevels.Add(dlcLevelInfo);

                    if (i + defaultLevelsCount < liveLevels.Count)
                    {
                        liveLevels[i + defaultLevelsCount] = dlcLevelInfo;
                    }
                    else
                    {
                        liveLevels.Add(dlcLevelInfo);
                    }
                }

                LevelsState.Instance.RewriteDLCLevelCache(dlcLevels);

                LevelsLoaded?.Invoke();
            }
            );
    }

    public static void AddOnLevelsLoadedListener(Action levelsLoaded)
    {
        LevelsLoaded += levelsLoaded;

        if(LevelCount > 0)
        {
            levelsLoaded?.Invoke();
        }
    }
    #endregion

    #region Data Access Methods
    public static void StartLevel(int level, out LevelInfo levelInfo)
    {
        if(IsLevelValid(level))
        {
            CurrentLevel = level;

            levelInfo = instance.liveLevels[level - 1];
        }
        else
        {
            throw DebugHelper.ThrowException("There is no level indexed with level " + level + ". " +
                "Ensure that level " + level + " is loaded.", instance.gameObject);
        }
    }

    public static void StartLevel(out LevelInfo levelInfo)
    {
        int nextLevel = LastLevelCompleted + 1;

        nextLevel = Mathf.Clamp(nextLevel, 1, LevelCount);

        CurrentLevel = nextLevel;

        levelInfo = instance.liveLevels[nextLevel - 1];
    }

    public static void StartInfiniteLevel()
    {
        CurrentLevel = int.MaxValue;
    }

    public static void StartTutorialLevel()
    {
        CurrentLevel = TUTORIAL_LEVEL;
    }

    public static bool IsLevelValid(int level)
    {
        int levelIndex = level - 1;

        return (level > 0 && levelIndex < instance.liveLevels.Count) || IsTutorialLevel(level);
    }

    public static bool IsLastLevel(int level)
    {
        return level == instance.liveLevels.Count;
    }

    public static bool IsLevelCompleted(int level)
    {
        return LevelsState.IsCompleted(level);
    }

    public static bool IsLevelUnlocked(int level)
    {
        return LevelsState.IsUnlocked(level) || IsTutorialLevel(level);
    }

    public static bool IsTutorialLevel(int level)
    {
        return level == TUTORIAL_LEVEL;
    }
    #endregion

    #region Action Methods
    public static int NextLevel()
    {
        if (IsTutorialLevel(CurrentLevel))
        {
            CurrentLevel = 1;
        }
        else
        {
            LastLevelCompleted = CurrentLevel;

            LevelsState.MarkCompleted(LastLevelCompleted);

            CurrentLevel++;

            CurrentLevel = Mathf.Clamp(CurrentLevel, 1, LevelCount);
        }

        bool previouslyFinishedIntro = LevelsState.IsUnlocked(6);

        LevelsState.MarkUnlocked(CurrentLevel);

        bool justfinishedIntro = LevelsState.IsUnlocked(6);

        if (!previouslyFinishedIntro && justfinishedIntro)
        {
            Analytics.LogIntroLevelsCompleted();
        }

        return CurrentLevel;
    }
    public static void Home()
    {
        CurrentLevel = 0;
    }
    #endregion
}
