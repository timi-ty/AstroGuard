﻿//In Progress
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
    public PowerUpOrbSpawner powerUpOrbSpawner;
    public BombSpawner bombSpawner;
    public GoldCoinSpawner coinSpawner;

    [Header("Debug Settings")]
    public bool muteBombs;
    public bool mutePowerUps;


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

    public void SpawnGoldCoin()
    {
        coinSpawner.SpawnGoldCoin();
    }

    public void SpawnGoldCoin(Vector2 position)
    {
        coinSpawner.SpawnGoldCoin(position);
    }
}
