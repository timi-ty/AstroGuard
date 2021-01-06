using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    #region Singleton
    public static TutorialManager instance { get; private set; }
    #endregion

    #region Tutorial Components
    private AsteroidCommander asteroidCommander => GameManager.instance.asteroidCoordinator;
    private SpawnerManager spawnerManager => GameManager.instance.spawnerCoordinator;
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
    #endregion

    public static void StartTutorial()
    {
        
    }

    public static void FinishTutorial()
    {

    }
}
