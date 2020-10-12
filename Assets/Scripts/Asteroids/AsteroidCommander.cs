//In Progress
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct AsteroidSpawnInfo
{
    public float spawnDelay;
    public int spawnPositionIndex;
    public int enemyType;
    public float enemySpeed;
    public float enemySize;
}


public class AsteroidCommander : MonoBehaviour
{
    #region Constants
    public const int SPAWN_POSITIONS_COUNT = 12;
    #endregion

    #region Prefabs
    [Header("Prefabs")]
    public List<AteroidBase> enemyPrefabs;
    #endregion

    #region Worker Parameters
    private int nEnemyLineup { get; set; } //total number of enemies for the current lineup
    private int nSpawnedAsteroids { get; set; }//number of enemies spawned so far
    private int nLiveAsteroids { get; set; } //current number of alive enemies in play
    private bool isDoneSpawning { get; set; }
    public PlayerBehaviour player { get; set; }
    #endregion

    #region Debug Parameters
    [Header("Debug Parameters")]
    public bool muteEnemyZero;
    public bool muteEnemyOne;
    public bool muteEnemyTwo;
    #endregion

    private void Start()
    {
        player = GameManager.instance.player;
    }

    public void OnPlay(List<AsteroidSpawnInfo> asteroidLineup)
    {
        nEnemyLineup = asteroidLineup.Count;
        nSpawnedAsteroids = 0;
        nLiveAsteroids = 0;

        StartCoroutine(SpawnRoutine(asteroidLineup));
    }

    public void OnPlay()
    {
        StartCoroutine(InfiniteSpawnRoutine());
    }


    private IEnumerator InfiniteSpawnRoutine()
    {
        nSpawnedAsteroids = 0;
        nLiveAsteroids = 0;
        int difficulty = 0;
        while (true)
        {
            isDoneSpawning = false;

            int length = UnityEngine.Random.Range(20, 500);
            float duration = UnityEngine.Random.Range(0.4f, 0.6f) * length;
            float maxDurationError = UnityEngine.Random.Range(0f, duration/5);


            List<AsteroidSpawnInfo> asteroidLineup = GetRanomAsteroidLineup(length, duration, maxDurationError, difficulty, LevelCollection.MAX_DIFFICULTY);

            nEnemyLineup += asteroidLineup.Count;

            difficulty = Mathf.Clamp(difficulty++, 0, LevelCollection.MAX_DIFFICULTY);

            StartCoroutine(SpawnRoutine(asteroidLineup));

            yield return new WaitUntil(() => isDoneSpawning);
        }
    }


    private IEnumerator SpawnRoutine(List<AsteroidSpawnInfo> enemyLineup)
    {
        for (int i = 0; i < enemyLineup.Count; i++)
        {
            AsteroidSpawnInfo enemyInfo = enemyLineup[i];
            float delay = enemyInfo.spawnDelay;
            yield return new WaitForSeconds(delay);

            SpawnEnemey(enemyInfo, i);
        }

        isDoneSpawning = true;
    }

    private void SpawnEnemey(AsteroidSpawnInfo spawnInfo, int spawnIndex)
    {
        if(enemyPrefabs.Count < 1)
        {
            DebugHelper.LogError("Enemy spawn failed. Check that enemyPrefabs list in AsteroidCommander is not empty.", gameObject);
            return;
        }

        int enemyTypeIndex = spawnInfo.enemyType;

        if(enemyTypeIndex < 0 || enemyTypeIndex >= enemyPrefabs.Count)
        {
            DebugHelper.LogWarning("AsteroidCommander recieved an invalid enemy type at spawnIndex " + spawnIndex + ". " +
                "Check that all needed enemy types have been assigned to the AsteroidCommander in the inspector. " +
                "Spawn defaulted to base enemy.", gameObject);

            enemyTypeIndex = 0;
        }

        Vector2 spawnPosition = new Vector2(ScreenBounds.SpecificXCoord(spawnInfo.spawnPositionIndex, SPAWN_POSITIONS_COUNT), ScreenBounds.max.y + 2);

        #region Mute Spawn For Debugging
        if ((enemyTypeIndex == 0 && muteEnemyZero) || (enemyTypeIndex == 1 && muteEnemyOne) || (enemyTypeIndex == 2 && muteEnemyTwo)) return;
        #endregion

        AteroidBase enemy = Instantiate(enemyPrefabs[enemyTypeIndex], spawnPosition, Quaternion.identity, transform);

        enemy.SetParams(enemyCommander: this,
                        speed: spawnInfo.enemySpeed,
                        size: spawnInfo.enemySize);

        nLiveAsteroids++;
        nSpawnedAsteroids++;
    }

    public void OnAsteroidDeath(AsteroidDeathInfo deathInfo, AteroidBase.AsteroidType enemyType, Vector2 position)
    {
        nLiveAsteroids--;

        Metrics.LogAsteroidDeath(deathInfo, enemyType);

        Session.RecordScoreBump();

        float destroyedRocks = nSpawnedAsteroids - nLiveAsteroids;
        float progress = destroyedRocks / nEnemyLineup;

        GameManager.UpdateLevelProgress(progress);

        switch (deathInfo.killer)
        {
            case AsteroidDeathInfo.Killer.Projectile:
            case AsteroidDeathInfo.Killer.Explosion:
            case AsteroidDeathInfo.Killer.PowerUpOrb:
                GameManager.SpawnGoldCoin(position);
                break;
        }

        if (isDoneSpawning && nLiveAsteroids <= 0)
        {
            Invoke("NotifyLevelComplete", 3);
        }

        ObjectiveManager.Refresh();
    }

    private void NotifyLevelComplete()
    {
        if (GameManager.isInInfiniteMode) return;

        GameManager.instance.OnLevelFinished();
    }

    #region Static Utility Methods
    public static List<AsteroidSpawnInfo> GetRanomAsteroidLineup(int length, float duration, float maxDurationError, int difficulty, int maxDifficulty)
    {
        List<AsteroidSpawnInfo> asteroidLineup = new List<AsteroidSpawnInfo>();

        float meanSpawnInterval = duration / length;
        float maxIntervalError = maxDurationError / length;

        for (int i = 0; i < length; i++)
        {
            AsteroidSpawnInfo spawnInfo = GetRandomAsteroidInfo(difficulty, maxDifficulty);

            float intervalError = UnityEngine.Random.Range(-maxIntervalError, maxIntervalError);

            spawnInfo.spawnDelay = Mathf.Clamp(meanSpawnInterval + intervalError, 0, meanSpawnInterval + maxIntervalError);

            asteroidLineup.Add(spawnInfo);
        }

        return asteroidLineup;
    }

    public static AsteroidSpawnInfo GetRandomAsteroidInfo(int difficulty, int maxDifficulty)
    {
        AsteroidSpawnInfo enemySpawnInfo = new AsteroidSpawnInfo();
        enemySpawnInfo.spawnPositionIndex = GetRandomAsteroidSpawnPositionIndex();
        enemySpawnInfo.enemySize = GetRandomAsteroidSize();
        enemySpawnInfo.enemySpeed = GetRandomAsteroidSpeed(difficulty, maxDifficulty);
        enemySpawnInfo.enemyType = GetRandomEnemyType(difficulty, maxDifficulty);
        return enemySpawnInfo;
    }

    private static int GetRandomAsteroidSpawnPositionIndex()
    {
        ScreenBounds.RandomXCoord(SPAWN_POSITIONS_COUNT, out int index);
        return index;
    }

    private static float GetRandomAsteroidSize()
    {
        return UnityEngine.Random.Range(0.8f, 1.2f); ;
    }

    private static float GetRandomAsteroidSpeed(int difficulty, int maxDifficulty)
    {
        return 1.0f + (difficulty / maxDifficulty) + UnityEngine.Random.value * ((difficulty / maxDifficulty * 2.5f) +  2.0f);
    }

    private static int GetRandomEnemyType(int difficulty, int maxDifficulty)
    {
        Vector3[] enemyTypeDistributions = new Vector3[] { new Vector3(24, 0, 0), new Vector3(20, 4, 0), new Vector3(17, 7, 0), new Vector3(17, 5, 2), new Vector3(15, 6, 3)
                                                         , new Vector3(13, 8, 3), new Vector3(11, 9, 4), new Vector3(10, 10, 4), new Vector3(10, 9, 5), new Vector3(10, 8, 6)
                                                         , new Vector3(9, 8, 7), new Vector3(5, 14, 5), new Vector3(2, 11, 11), new Vector3(0, 24, 0), new Vector3(0, 0, 24)};

        int index = Mathf.RoundToInt((difficulty / maxDifficulty) * (enemyTypeDistributions.Length - 1));

        Vector3 distribution = enemyTypeDistributions[index];

        int enemyTypeDecider = UnityEngine.Random.Range(0, 24);

        if (enemyTypeDecider >= distribution.x + distribution.y && enemyTypeDecider < distribution.x + distribution.y + distribution.z)
        {
            return 2;
        }
        else if (enemyTypeDecider >= distribution.x && enemyTypeDecider < distribution.x + distribution.y)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    #endregion
}