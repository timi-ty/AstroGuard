//In Progress
using UnityEngine;
using System.Collections.Generic;

public abstract class OutsideSpawnerBase<T> : MonoBehaviour
{
    public List<T> prefabs = new List<T>();

    public abstract void StartSpawning<J>(List<J> spawnLineup, float extraSpawnGenerosity);
}
