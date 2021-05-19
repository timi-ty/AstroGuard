using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
public class LevelShrinker : MonoBehaviour
{
    #region Constants
    public const float MIN_LEVEL_DUR_LIMIT = 5;
    public const float MAX_LEVEL_DUR_LIMIT = 40;
    #endregion

    #region Data
    public LevelCollection levelCollection;
    private List<LevelInfo> shrunkLevels;
    public int ShrunkLevelsCount => shrunkLevels.Count;
    #endregion

    #region Worker Parameters
    public float minLevelDurVal;
    public float maxLevelDurVal;
    public int startLevel = 1;
    public int endLevel = 9;
    #endregion


    public void ParseLevels()
    {
        if (!levelCollection)
        {
            Debug.Log("Couldn't find a LevelCollection to parse");
            return;
        }
        shrunkLevels = levelCollection.GetAll();

        Debug.Log("Pulled All Levels From Collection.");
    }

    public void ShrinkLevels()
    {
        for (int i = startLevel - 1; i < endLevel; i++)
        {
            LevelInfo levelInfo = shrunkLevels[i];
            List<AsteroidSpawnInfo> shrunkAsteroidLineup = new List<AsteroidSpawnInfo>();
            float duration = 0;
            float targetDuration = Random.Range(minLevelDurVal, maxLevelDurVal);
            foreach (AsteroidSpawnInfo asteroidSpawnInfo in levelInfo.enemyLineup)
            {
                duration += asteroidSpawnInfo.spawnDelay;
                shrunkAsteroidLineup.Add(asteroidSpawnInfo);

                if(duration >= targetDuration)
                {
                    break;
                }
            }

            levelInfo.enemyLineup = shrunkAsteroidLineup;
            shrunkLevels[i] = levelInfo;

            Debug.Log("Shrunk Level " + (i + 1) + " to " + targetDuration + " Seconds.");
        }

        Debug.Log("Shrunk Levels " + startLevel + " - " + endLevel + " to between " + 
            minLevelDurVal + " - " + maxLevelDurVal + " Secs.");
    }

    public void PublishShrunkLevels()
    {
        for (int i = 0; i < shrunkLevels.Count; i++)
        {
            LevelInfo levelInfo = shrunkLevels[i];

            levelCollection.OverwriteAsDefaultLevel(levelInfo, i);
        }

        Debug.Log("Successfully Published All Shrunk Levels.");
    }
}
#endif