//In Progress
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelCollection : ScriptableObject
{
    #region Constants
    public const int MAX_DIFFICULTY = 30;
    #endregion

    #region Data
    public List<string> defaultLevelCollection = new List<string>();
    private List<string> tempUnshuffledLevelCollection = new List<string>();
    #endregion

    #region Properties
    public int Count => defaultLevelCollection.Count;
    #endregion

#if UNITY_EDITOR
    #region Editor Cache
    public List<LevelInfo> _levelInfoCollection = new List<LevelInfo>();
    public List<float> _durations = new List<float>();
    public LevelInfo _levelInfo = new LevelInfo();
    public AsteroidSpawnInfo _enemySpawnInfo = new AsteroidSpawnInfo();
    public PowerUpOrbSpawnInfo _powerUpOrbSpawnInfo = new PowerUpOrbSpawnInfo();
    public BombSpawnInfo _bombSpawnInfo = new BombSpawnInfo();
    public float _powerUpSpawnDuration;
    public int shuffleStartLevel;
    public int shuffleEndLevel;
    #endregion

    private void Awake()
    {
        _levelInfo = new LevelInfo();
        _levelInfo.enemyLineup = new List<AsteroidSpawnInfo>();
        _levelInfo.objectSpawnSettings = new ObjectSpawnSettings();
        _levelInfo.objectSpawnSettings.powerUpOrbLineup = new List<PowerUpOrbSpawnInfo>();
        _levelInfo.objectSpawnSettings.bombLineup = new List<BombSpawnInfo>();
    }

    #region Level Collection Editor Methods
    public void SaveAsDefaultLevel()
    {
        defaultLevelCollection.Add(CurrentLevelInfoAsJson());

        EditorUtility.SetDirty(this);

        LoadLevelInfoCollection();

        Debug.Log("New Default Level Saved. Level " + (defaultLevelCollection.Count));
    }

    public void LoadLevelInfoCollection()
    {
        _levelInfoCollection = GetAll();
        _durations = new List<float>();
        foreach(LevelInfo levelInfo in _levelInfoCollection)
        {
            float duration = 0;
            foreach(AsteroidSpawnInfo asteroidSpawnInfo in levelInfo.enemyLineup)
            {
                duration += asteroidSpawnInfo.spawnDelay;
            }
            _durations.Add(duration);
        }
        Debug.Log("Editor level list refreshed");
    }

    public void InsertLevel(int index)
    {
        defaultLevelCollection.Insert(index + 1, CurrentLevelInfoAsJson());

        LoadLevelInfoCollection();

        EditorUtility.SetDirty(this);

        Debug.Log("Level " + (index + 2) + " Inserted.");
    }

    private string CurrentLevelInfoAsJson()
    {
        LevelInfo levelInfo = _levelInfo;

        int backgroundIndex = levelInfo.environmentSettings.backgroundAssetIndex;

        if(backgroundIndex < 0) 
        {
            levelInfo.environmentSettings.backgroundAssetIndex = Background.GetRandomBackgroundIndex();
        }

        return JsonUtility.ToJson(levelInfo, true);
    }

    public void DeleteLevel(int index)
    {
        defaultLevelCollection.RemoveAt(index);

        LoadLevelInfoCollection();

        EditorUtility.SetDirty(this);

        Debug.Log("Level " + (index + 1) + " Deleted.");
    }

    public void ClearAll()
    {
        defaultLevelCollection.Clear();

        LoadLevelInfoCollection();

        EditorUtility.SetDirty(this);

        Debug.Log(name + " Cleared!");
    }

    public void ShuffleCollection()
    {
        if (shuffleEndLevel <= shuffleStartLevel || shuffleEndLevel > defaultLevelCollection.Count || shuffleStartLevel <= 0)
        {
            Debug.LogWarning("Invalid shuffle parameters. Level shuffle did not take place.");
            return;
        }

        tempUnshuffledLevelCollection = new List<string>();
        tempUnshuffledLevelCollection.AddRange(defaultLevelCollection);

        defaultLevelCollection.Shuffle(new System.Random(), shuffleStartLevel - 1, shuffleEndLevel - 1);

        LoadLevelInfoCollection();

        EditorUtility.SetDirty(this);

        Debug.Log(string.Format("Levels {0} to {1} shuffled.", shuffleStartLevel, shuffleEndLevel));
    }

    public void UndoShuffle()
    {
        if(tempUnshuffledLevelCollection.Count != defaultLevelCollection.Count)
        {
            Debug.LogWarning("No shuffle to undo.");
            return;
        }

        defaultLevelCollection = tempUnshuffledLevelCollection;

        LoadLevelInfoCollection();

        EditorUtility.SetDirty(this);

        Debug.Log("Last shuffle undone.");
    }

    public void SortLevels()
    {
        int i, key, j;
        for (i = 1; i < defaultLevelCollection.Count; i++)
        {
            string keyString = defaultLevelCollection[i];
            key = defaultLevelCollection[i].Length;
            j = i - 1;

            /* Move elements of arr[0..i-1], that are  
            greater than key, to one position ahead  
            of their current position */
            while (j >= 0 && defaultLevelCollection[j].Length > key)
            {
                defaultLevelCollection[j + 1] = defaultLevelCollection[j];
                j -= 1;
            }
            defaultLevelCollection[j + 1] = keyString;
        }

        LoadLevelInfoCollection();

        EditorUtility.SetDirty(this);

        Debug.Log("Levels Sorted.");
    }
    #endregion

    #region Random Level Generator Methods
    public void AddRandomEnemyLineup()
    {
        int length = (int) _enemySpawnInfo.spawnDelay;
        int difficulty = Mathf.Clamp(_enemySpawnInfo.enemyType, 0, MAX_DIFFICULTY);
        float duration = _enemySpawnInfo.enemySpeed;
        float maxDurationError = _enemySpawnInfo.enemySize;

        _levelInfo.enemyLineup.AddRange(
            AsteroidCommander.GetRandomAsteroidLineup(length: length,
                                               duration: duration,
                                               maxDurationError: maxDurationError,
                                               difficulty: difficulty,
                                               maxDifficulty: MAX_DIFFICULTY));
    }

    public void AddRandomPowerUpLineup()
    {
        int length = _powerUpOrbSpawnInfo.spawnSide;
        float duration = _powerUpSpawnDuration;
        float maxDurationError = _powerUpOrbSpawnInfo.spawnDelay;

        _levelInfo.objectSpawnSettings.powerUpOrbLineup.AddRange(
            PowerUpOrbSpawner.GetRanomPowerUpOrbLineup(length, duration, maxDurationError));
    }

    public void AddRandomBombLineup()
    {
        int length = _bombSpawnInfo.spawnSide;
        float duration = _bombSpawnInfo.bombSize;
        float maxDurationError = _bombSpawnInfo.spawnDelay;

        _levelInfo.objectSpawnSettings.bombLineup.AddRange(
            BombSpawner.GetRanomBombLineup(length, duration, maxDurationError));
    }

    public void AssignRandomBGIndeces()
    {
        for (int i = 0; i < defaultLevelCollection.Count; i++)
        {
            LevelInfo level = JsonUtility.FromJson<LevelInfo>(defaultLevelCollection[i]);

            level.environmentSettings.backgroundAssetIndex = Background.GetRandomBackgroundIndex();

            defaultLevelCollection[i] = JsonUtility.ToJson(level, true);
        }

        EditorUtility.SetDirty(this);

        Debug.Log("Background Indeces Randomized.");
    }
    #endregion


    [MenuItem("Assets/Create/Level Collection")]
    public static void CreateDefaultLevels()
    {
        string path = EditorUtility.SaveFilePanelInProject("New Default Levels", "DefaultLevels", "Asset", "Save Default Levels", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(CreateInstance<LevelCollection>(), path);
    }
#endif

    #region Data Access Methods
    public List<LevelInfo> GetAll()
    {
        List<LevelInfo> defaultLevelsInfo = new List<LevelInfo>();

        foreach (string levelJSON in defaultLevelCollection)
        {
            LevelInfo levelInfo = JsonUtility.FromJson<LevelInfo>(levelJSON);

            defaultLevelsInfo.Add(levelInfo);
        }

        return defaultLevelsInfo;
    }
    #endregion
}