//In Progress
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelCollection : ScriptableObject
{
    #region Constants
    public const int MAX_DIFFICULTY = 20;
    #endregion

    #region Data
    public List<string> defaultLevelCollection = new List<string>();
    #endregion

    #region Properties
    public int Count => defaultLevelCollection.Count;
    #endregion

#if UNITY_EDITOR
    #region Editor Cache
    public LevelInfo _levelInfo = new LevelInfo();
    public AsteroidSpawnInfo _enemySpawnInfo = new AsteroidSpawnInfo();
    public PowerUpOrbSpawnInfo _powerUpOrbSpawnInfo = new PowerUpOrbSpawnInfo();
    public BombSpawnInfo _bombSpawnInfo = new BombSpawnInfo();
    public float _powerUpSpawnDuration;
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

        Debug.Log("New Default Level Saved. Level " + (defaultLevelCollection.Count));
    }

    public void InsertLevel(int index)
    {
        defaultLevelCollection.Insert(index + 1, CurrentLevelInfoAsJson());

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

        EditorUtility.SetDirty(this);

        Debug.Log("Level " + (index + 1) + " Deleted.");
    }

    public void ClearAll()
    {
        defaultLevelCollection.Clear();

        EditorUtility.SetDirty(this);

        Debug.Log(name + " Cleared!");
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
            AsteroidCommander.GetRanomEnemyLineup(length: length,
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