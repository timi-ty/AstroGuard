using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoldCoinSpawner : OutsideSpawnerBase<GoldCoin>
{
    internal Vector2[] spawnPoints = new Vector2[2];


    private void Start()
    {
        spawnPoints[0] = ScreenBounds.leftEdge.middle + Vector2.left;
        spawnPoints[1] = ScreenBounds.rightEdge.middle + Vector2.right;
    }

    public void SpawnGoldCoin()
    {
        int spawnPoint = Random.Range(0, 2);

        Vector3 pos = new Vector3(spawnPoints[spawnPoint].x, spawnPoints[spawnPoint].y, transform.position.z);

        Instantiate(prefabs[0], pos, Quaternion.identity, transform);
    }

    public void SpawnGoldCoin(Vector2 position)
    {
        Vector3 pos = new Vector3(position.x, position.y, transform.position.z);

        Instantiate(prefabs[0], pos, Quaternion.identity, transform);
    }

    public override void StartSpawning<J>(List<J> spawnLineup)
    {
        //Do Nothing
    }

    public override void StartSpawning()
    {
        //Do Nothing
    }

    protected override void PreSpawn()
    {
        //Do Nothing
    }
}
