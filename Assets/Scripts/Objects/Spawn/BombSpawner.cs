using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct BombSpawnInfo
{
    public float spawnDelay;
    public int spawnSide;
    public float bombSize;
}

public class BombSpawner : OutsideSpawnerBase<Bomb>
{
    #region Internal
    internal Vector2 bombSize;
    internal Vector2[] spawnPoints = new Vector2[2];
    #endregion


    #region Abstract Methods
    public override void StartSpawning<J>(List<J> bombLineup, float extraSpawnGenerosity)
    {
        if (prefabs.Count > 0)
        {
            bombSize = prefabs[0].GetComponentInChildren<Collider2D>().bounds.size * bombSize;
        }

        spawnPoints[0] = ScreenBounds.leftEdge.middle + Vector2.up * ScreenBounds.height/4 + Vector2.left * bombSize.x;
        spawnPoints[1] = ScreenBounds.rightEdge.middle + Vector2.up * ScreenBounds.height / 4 + Vector2.right * bombSize.x;

        StartCoroutine(SpawnRoutine(bombLineup));
    }
    #endregion

    #region Sealed Methods
    private IEnumerator SpawnRoutine<J>(List<J> bombSpawnLineup)
    {
        for (int i = 0; i < bombSpawnLineup.Count; i++)
        {
            BombSpawnInfo spawnInfo = (BombSpawnInfo)(object)bombSpawnLineup[i];
            float delay = spawnInfo.spawnDelay;
            yield return new WaitForSeconds(delay);

            SpawnBomb(spawnInfo);
        }
    }

    private void SpawnBomb(BombSpawnInfo spawnInfo)
    {
        if(prefabs.Count != 1)
        {
            throw DebugHelper.ThrowException("The bomb spawner can only work with one and only one bomb prefab.", gameObject);
        }

        spawnInfo.spawnSide = Mathf.Clamp(spawnInfo.spawnSide, 0, 1);

        Bomb bomb = Instantiate(prefabs[0], spawnPoints[spawnInfo.spawnSide], Quaternion.identity, transform);
        bomb.SetSize(spawnInfo.bombSize);
    }
    #endregion

    #region Static Utility Methods
    public static List<BombSpawnInfo> GetRanomBombLineup(int length, float duration, float maxDurationError)
    {
        List<BombSpawnInfo> bombLineup = new List<BombSpawnInfo>();

        float meanSpawnInterval = duration / length;
        float maxIntervalError = maxDurationError / length;

        for (int i = 0; i < length; i++)
        {
            BombSpawnInfo spawnInfo;

            float intervalError = Random.Range(-maxIntervalError, maxIntervalError);

            spawnInfo.spawnDelay = Mathf.Clamp(meanSpawnInterval + intervalError, 0, meanSpawnInterval + maxIntervalError);

            spawnInfo.spawnSide = Random.Range(0, 2);

            spawnInfo.bombSize = Random.Range(0.8f, 1.0f);

            bombLineup.Add(spawnInfo);
        }

        return bombLineup;
    }
    #endregion
}
