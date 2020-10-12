//In Progress
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct PowerUpOrbSpawnInfo
{
    public float spawnDelay;
    public int spawnSide;
    public PowerType powerType;
}

public class PowerUpOrbSpawner : OutsideSpawnerBase<PowerUpOrb>
{

    #region Worker Parameters
    private Vector2 powerUpOrbSize;
    private Vector2[] spawnPoints = new Vector2[2];
    private bool isDoneSpawning { get; set; }
    #endregion

    #region Probability Parameters
    private const float SPAWN_ATTEMPT_INTERVAL = 3.0f;
    private int spawnSuccessOdds;
    #endregion

    #region Abstract Methods
    public override void StartSpawning<J>(List<J> powerUpOrbLineup)
    {
        PreSpawn();

        StartCoroutine(SpawnRoutine(powerUpOrbLineup));
    }

    public override void StartSpawning()
    {
        PreSpawn();

        StartCoroutine(InfiniteSpawnRoutine());
    }

    protected override void PreSpawn()
    {
        if (prefabs.Count > 0)
        {
            powerUpOrbSize = prefabs[0].GetComponentInChildren<Collider2D>().bounds.size;
        }

        spawnPoints[0] = ScreenBounds.leftEdge.middle + Vector2.left * powerUpOrbSize.x;
        spawnPoints[1] = ScreenBounds.rightEdge.middle + Vector2.right * powerUpOrbSize.x;
    }
    #endregion

    #region Sealed Methods

    private IEnumerator SpawnRoutine<J>(List<J> powerUpOrbLineup)
    {
        for (int i = 0; i < powerUpOrbLineup.Count; i++)
        {
            PowerUpOrbSpawnInfo spawnInfo = (PowerUpOrbSpawnInfo)(object)powerUpOrbLineup[i];
            float delay = spawnInfo.spawnDelay;
            yield return new WaitForSeconds(delay);

            SpawnPowerUp(spawnInfo);
        }

        isDoneSpawning = true;
    }

    private IEnumerator InfiniteSpawnRoutine()
    {
        while (true)
        {
            isDoneSpawning = false;

            int length = Random.Range(5, 20);
            float duration = Random.Range(5f, 20f) * length;
            float maxDurationError = Random.Range(0f, duration / 5);


            List<PowerUpOrbSpawnInfo> powerUpLineup = GetRanomPowerUpOrbLineup(length, duration, maxDurationError);

            StartCoroutine(SpawnRoutine(powerUpLineup));

            yield return new WaitUntil(() => isDoneSpawning);
        }
    }

    private void SpawnPowerUp(PowerUpOrbSpawnInfo spawnInfo)
    {
        spawnInfo.spawnSide = Mathf.Clamp(spawnInfo.spawnSide, 0, 1);

        Instantiate(GetPowerUpOrbPrefab(spawnInfo.powerType), spawnPoints[spawnInfo.spawnSide], Quaternion.identity, transform);
    }

    private PowerUpOrb GetPowerUpOrbPrefab(PowerType powerType)
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            if(powerType == prefabs[i].powerType)
            {
                return prefabs[i];
            }
        }

        throw DebugHelper.ThrowException("Could not find a corresponding prefab for this PowerType: " + powerType.ToString(), gameObject);
    }

    #region Random Spawning Methods
    private void PowerUpOrbSpawnLottery()
    {
        int spawnLottery = Random.Range(0, spawnSuccessOdds);

        PowerUpOrbSpawnInfo spawnInfo = new PowerUpOrbSpawnInfo();

        spawnInfo.powerType = (PowerType) Random.Range(0, prefabs.Count);

        spawnInfo.spawnSide = Random.Range(0, 2);

        if (spawnLottery == 0)
        {
            SpawnPowerUp(spawnInfo);
        }
    }

    private void ComputeSpawnSuccessOdds(float extraSpawnGenerosity)
    {
        float intervalForNearlyCertainSpawn = 10.0f / extraSpawnGenerosity;

        float spawnSuccessProbability = SPAWN_ATTEMPT_INTERVAL / intervalForNearlyCertainSpawn;

        spawnSuccessOdds = Mathf.RoundToInt(1 / spawnSuccessProbability);
    }
    #endregion

    #endregion

    #region Static Utility Methods
    public static List<PowerUpOrbSpawnInfo> GetRanomPowerUpOrbLineup(int length, float duration, float maxDurationError)
    {
        List<PowerUpOrbSpawnInfo> PowerUpOrbLineup = new List<PowerUpOrbSpawnInfo>();

        float meanSpawnInterval = duration / length;
        float maxIntervalError = maxDurationError / length;

        for (int i = 0; i < length; i++)
        {
            PowerUpOrbSpawnInfo spawnInfo = new PowerUpOrbSpawnInfo();

            float intervalError = Random.Range(-maxIntervalError, maxIntervalError);

            spawnInfo.spawnDelay = Mathf.Clamp(meanSpawnInterval + intervalError, 0, meanSpawnInterval + maxIntervalError);

            spawnInfo.powerType = (PowerType)Random.Range(0, 4);

            spawnInfo.spawnSide = Random.Range(0, 2);

            PowerUpOrbLineup.Add(spawnInfo);
        }

        return PowerUpOrbLineup;
    }
    #endregion
}
