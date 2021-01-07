//In Progress
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct ObjectSpawnSettings
{
    public float powerUpOrbGenerosity;
    public List<PowerUpOrbSpawnInfo> powerUpOrbLineup;
    public List<BombSpawnInfo> bombLineup;
}

public class SpawnerManager : MonoBehaviour
{
    #region Singleton
    public static SpawnerManager instance { get; private set; }
    #endregion

    #region Components
    public PowerUpOrbSpawner powerUpOrbSpawner;
    public BombSpawner bombSpawner;
    public GoldCoinSpawner coinSpawner;
    #endregion

    #region Debug
    [Header("Debug Settings")]
    public bool muteBombs;
    public bool mutePowerUps;
    #endregion

    #region Unity Runtime
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

    private void OnDestroy()
    {
        #region Singleton
        instance = null;
        #endregion
    }
#endregion

    public void OnPlay(ObjectSpawnSettings objectSpawnSettings)
    {
        if (!mutePowerUps) powerUpOrbSpawner.StartSpawning(objectSpawnSettings.powerUpOrbLineup);
        if (!muteBombs) bombSpawner.StartSpawning(objectSpawnSettings.bombLineup);
    }

    public void OnPlay()
    {
        if (!mutePowerUps) powerUpOrbSpawner.StartSpawning();
        if (!muteBombs) bombSpawner.StartSpawning();
    }

    public static void SpawnGoldCoin()
    {
        if (!instance) return;

        instance.coinSpawner.SpawnGoldCoin();
    }

    public static void SpawnGoldCoin(Vector2 position)
    {
        if (!instance) return;

        instance.coinSpawner.SpawnGoldCoin(position);
    }

    public Bomb SpawnBomb()
    {
        if (!instance) return null;

        return bombSpawner.SpawnBomb(new BombSpawnInfo(0, UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0.8f, 1.0f)));
    }

    public PowerUpOrb SpawnPowerUp(PowerType powerType)
    {
        if (!instance) return null;

        return powerUpOrbSpawner.SpawnPowerUp(new PowerUpOrbSpawnInfo(0, UnityEngine.Random.Range(0, 2), powerType));
    }
}
