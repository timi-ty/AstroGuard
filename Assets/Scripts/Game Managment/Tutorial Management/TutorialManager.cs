using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    #region Singleton
    public static TutorialManager instance { get; private set; }
    #endregion

    #region Tutorial Components
    public TutorialUI tutorialUI;
    private ProximitySensor proximitySensor => GameManager.instance.ship.proximitySensor;
    private AsteroidCommander asteroidCommander => GameManager.instance.asteroidCoordinator;
    private SpawnerManager spawnerManager => GameManager.instance.spawnerCoordinator;
    private PlayerBehaviour player => GameManager.instance.player;
    #endregion

    #region Worker Parameters
    private bool shouldProceed;
    private int tutorialSequenceSize;
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

    private void Start()
    {
        instance.tutorialUI.gameObject.SetActive(false);
        tutorialSequenceSize = 12;
    }

    public static void StartTutorial()
    {
        instance.tutorialUI.gameObject.SetActive(true);
        instance.tutorialUI.SetInteraction(() => instance.shouldProceed = true);
        instance.StartCoroutine(instance.RunTutorialSequence());

        UIManager.instance.playHud.ShowPauseButton(false);
    }

    public static void FinishTutorial()
    {
        PlayerStats.MarkTutorialFinished();

        UIManager.instance.playHud.ShowPauseButton(true);

        GameManager.instance.OnLevelFinished();
    }

    #region Tutorial Sequence
    private IEnumerator RunTutorialSequence()
    {
        yield return null;
        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.ShowFirstPrompt(() => instance.tutorialUI.CanProceed(true));//first inro message

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        GameManager.UpdateLevelProgress(1.0f / tutorialSequenceSize, true);

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.ShowSecondPrompt(() => instance.tutorialUI.CanProceed(true));//second intro message

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        GameManager.UpdateLevelProgress(2.0f / tutorialSequenceSize, true);

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.ShowThirdPrompt(
            onPromptShowed: () =>
            {
                instance.tutorialUI.CanProceed(true, "Press and Hold");//control instuctions prompt
                player.OnPlay();//enable control
            });

        yield return new WaitUntil(() => player.enabled);

        //listen for Press and Hold
        float holdTime = 0;
        while(holdTime < 2)
        {
            if (holdTime >= 2) break;
            if(IsHoldingScreen()) holdTime += Time.deltaTime;
            tutorialUI.SetActionProgress(holdTime / 2.0f);
            yield return null;
        }
        shouldProceed = false;

        AudioManager.instance.PlayUIButtonSFX();

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

        GameManager.UpdateLevelProgress(3.0f / tutorialSequenceSize, true);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Destroying Asteroid Zero

        while (true)
        {
            AsteroidBase asteroid = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.Zero, 2.5f, 1.0f)); //spawn an asteroid
            yield return new WaitUntil(() => proximitySensor.IsInProximity(asteroid.GetComponentInChildren<Collider2D>()));//wait for asteroid to come close

            Time.timeScale = 0.02f; //slow down time to give instructions
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            instance.tutorialUI.gameObject.SetActive(true);//tutorial UI re-appears
            instance.tutorialUI.ShowFourthPrompt(() => shouldProceed = true);//destroy asteroid prompt

            yield return new WaitUntil(() => shouldProceed);
            yield return new WaitForSecondsRealtime(1.0f);
            shouldProceed = false;

            instance.tutorialUI.CanProceed(false);
            instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

            yield return new WaitForSecondsRealtime(0.2f);

            Time.timeScale = 1.0f; //restore time scale
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return new WaitUntil(() => asteroid == null);//wait for asteroid to be destroyed

            if (AsteroidCommander.lastAsteroidKiller == AsteroidDeathInfo.Killer.Sword) break;//proceed if asteroid was destroyed by the player's sword

            yield return new WaitForSecondsRealtime(3.0f);
        }

        GameManager.UpdateLevelProgress(4.0f / tutorialSequenceSize, true);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Destroying Asteroid One

        player.OnResume();//stop the player's sword forom accelerating;
        player.OnPause();//stop the player from moving

        instance.tutorialUI.gameObject.SetActive(true);//tutorial UI re-appears
        instance.tutorialUI.ShowFifthPrompt(() => instance.tutorialUI.CanProceed(true));//congrats message

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.ShowSixthPrompt(() => instance.tutorialUI.CanProceed(true));//blue asteroids info

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

        player.OnResume();

        while (true)
        {
            AsteroidBase asteroid = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.One, 2.5f, 1.0f)); //spawn the blue asteroid

            yield return new WaitUntil(() => asteroid == null); //wait for asteroid to be destroyed

            if (AsteroidCommander.lastAsteroidKiller == AsteroidDeathInfo.Killer.Sword) break; //proceed if asteroid was destroyed by the player's sword

            yield return new WaitForSecondsRealtime(3.0f);
        }

        GameManager.UpdateLevelProgress(5.0f / tutorialSequenceSize, true);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Destroying Asteroid Two

        player.OnResume();//stop the player's sword forom accelerating;
        player.OnPause();//stop the player from moving

        instance.tutorialUI.gameObject.SetActive(true);//tutorial UI re-appears
        instance.tutorialUI.ShowSeventhPrompt(() => instance.tutorialUI.CanProceed(true));//congrats message

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.ShowEigthPrompt(() => instance.tutorialUI.CanProceed(true));//purple asteroids info

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

        player.OnResume();

        while (true)
        {
            AsteroidBase asteroid = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.Two, 3.5f, 1.0f)); //spawn the purple asteroid

            yield return new WaitUntil(() => asteroid == null); //wait for asteroid to be destroyed

            if (AsteroidCommander.lastAsteroidKiller == AsteroidDeathInfo.Killer.Sword) break; //proceed if asteroid was destroyed by the player's sword

            yield return new WaitForSecondsRealtime(3.0f);
        }

        GameManager.UpdateLevelProgress(6.0f / tutorialSequenceSize, true);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Destroying Bomb

        player.OnResume();//stop the player's sword forom accelerating;
        player.OnPause();//stop the player from moving

        instance.tutorialUI.gameObject.SetActive(true);//tutorial UI re-appears
        instance.tutorialUI.ShowNinthPrompt(() => instance.tutorialUI.CanProceed(true));//congrats message

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.ShowTenthPrompt(() => instance.tutorialUI.CanProceed(true));//bombs info

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

        player.OnResume();

        while (true)
        {
            Bomb bomb = spawnerManager.SpawnBomb();

            AsteroidBase asteroid = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.Zero, 2.5f, 1.0f)); //spawn an asteroid

            yield return new WaitUntil(() => bomb == null); //wait for bomb to be destroyed
            yield return new WaitUntil(() => asteroid == null); //wait for asteroid to be destroyed

            if (AsteroidCommander.lastAsteroidKiller == AsteroidDeathInfo.Killer.Explosion 
                && Bomb.lastBombTrigger == BombExplosionInfo.Trigger.Sword 
                || Bomb.lastBombTrigger == BombExplosionInfo.Trigger.Player) break; //proceed if asteroid was destroyed by the bomb's explosion

            yield return new WaitForSecondsRealtime(3.0f);
        }

        GameManager.UpdateLevelProgress(7.0f / tutorialSequenceSize, true);

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Getting Shield

        player.OnResume();//stop the player's sword forom accelerating;
        player.OnPause();//stop the player from moving

        instance.tutorialUI.gameObject.SetActive(true);//tutorial UI re-appears
        instance.tutorialUI.ShowEleventhPrompt(() => instance.tutorialUI.CanProceed(true));//congrats message

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.ShowTwelfthPrompt(() => instance.tutorialUI.CanProceed(true));//shield info

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

        player.OnResume();

        while (true)
        {
            PowerUpOrb powerUpOrb = spawnerManager.SpawnPowerUp(PowerType.Shield);

            AsteroidBase asteroid0 = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
                AsteroidBase.AsteroidType.Zero, 1.5f, 1.0f));

            AsteroidBase asteroid1 = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.One, 1.5f, 1.0f));

            AsteroidBase asteroid2 = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.Two, 1.5f, 1.0f));

            yield return new WaitUntil(() => asteroid0 == null && asteroid1 == null && asteroid2 == null);//wait for asteroids to be destroyed

            if (powerUpOrb == null) break; //proceed if shield power-up was gotten

            yield return new WaitForSecondsRealtime(3.0f);
        }

        GameManager.UpdateLevelProgress(8.0f / tutorialSequenceSize, true);

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Getting Attractor

        player.OnResume();//stop the player's sword forom accelerating;
        player.OnPause();//stop the player from moving

        instance.tutorialUI.gameObject.SetActive(true);//tutorial UI re-appears
        instance.tutorialUI.ShowThirteenthPrompt(() => instance.tutorialUI.CanProceed(true));//attractor info

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

        player.OnResume();

        while (true)
        {
            PowerUpOrb powerUpOrb = spawnerManager.SpawnPowerUp(PowerType.Attractor);

            AsteroidBase asteroid0 = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
                AsteroidBase.AsteroidType.Zero, 1.5f, 1.0f));

            AsteroidBase asteroid1 = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.One, 1.5f, 1.0f));

            AsteroidBase asteroid2 = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.Two, 1.5f, 1.0f));

            yield return new WaitUntil(() => asteroid0 == null && asteroid1 == null && asteroid2 == null);//wait for asteroids to be destroyed

            if (powerUpOrb == null) break; //proceed if shield power-up was gotten

            yield return new WaitForSecondsRealtime(3.0f);
        }

        GameManager.UpdateLevelProgress(9.0f / tutorialSequenceSize, true);

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Getting Missile

        player.OnResume();//stop the player's sword forom accelerating;
        player.OnPause();//stop the player from moving

        instance.tutorialUI.gameObject.SetActive(true);//tutorial UI re-appears
        instance.tutorialUI.ShowFourteenthPrompt(() => instance.tutorialUI.CanProceed(true));//missile launcher info

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

        player.OnResume();

        while (true)
        {
            PowerUpOrb powerUpOrb = spawnerManager.SpawnPowerUp(PowerType.MissileLauncher);

            AsteroidBase asteroid0 = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
                AsteroidBase.AsteroidType.Zero, 1.5f, 1.0f));

            AsteroidBase asteroid1 = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.One, 1.5f, 1.0f));

            AsteroidBase asteroid2 = asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.Two, 1.5f, 1.0f));

            yield return new WaitUntil(() => asteroid0 == null && asteroid1 == null && asteroid2 == null);//wait for asteroids to be destroyed

            if (powerUpOrb == null) break; //proceed if shield power-up was gotten

            yield return new WaitForSecondsRealtime(3.0f);
        }

        GameManager.UpdateLevelProgress(10.0f / tutorialSequenceSize, true);

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Getting SlowMo

        player.OnResume();//stop the player's sword forom accelerating;
        player.OnPause();//stop the player from moving

        instance.tutorialUI.gameObject.SetActive(true);//tutorial UI re-appears
        instance.tutorialUI.ShowFifteenthPrompt(() => instance.tutorialUI.CanProceed(true));//slowmo info

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

        player.OnResume();

        while (true)
        {
            AsteroidZero asteroid0 = (AsteroidZero) asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, 1, (int)
                AsteroidBase.AsteroidType.Zero, 3.5f, 1.0f));

            AsteroidZero asteroid1 = (AsteroidZero) asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT / 2, (int)
            AsteroidBase.AsteroidType.Zero, 3.5f, 1.0f));

            AsteroidZero asteroid2 = (AsteroidZero)asteroidCommander.SpawnEnemy(new AsteroidSpawnInfo(0, AsteroidCommander.SPAWN_POSITIONS_COUNT, (int)
            AsteroidBase.AsteroidType.Zero, 3.5f, 1.0f));

            asteroid0.ManualTargetPoint(new Vector2(ScreenBounds.SpecificXCoord(1, 15), ScreenBounds.centre.y));
            asteroid1.ManualTargetPoint(new Vector2(ScreenBounds.SpecificXCoord(7, 15), ScreenBounds.centre.y));
            asteroid2.ManualTargetPoint(new Vector2(ScreenBounds.SpecificXCoord(15, 15), ScreenBounds.centre.y));

            yield return new WaitUntil(() => asteroid0 == null && asteroid1 == null && asteroid2 == null);//wait for asteroids to be destroyed

            if (AsteroidCommander.lastAsteroidKiller == AsteroidDeathInfo.Killer.Sword) break; //proceed if asteroid was destroyed by the player

            yield return new WaitForSecondsRealtime(3.0f);
        }

        GameManager.UpdateLevelProgress(11.0f / tutorialSequenceSize, true);

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Last Message

        player.OnResume();//stop the player's sword forom accelerating;
        player.OnPause();//stop the player from moving

        instance.tutorialUI.gameObject.SetActive(true);//tutorial UI re-appears
        instance.tutorialUI.ShowSixteenthPrompt(() => instance.tutorialUI.CanProceed(true));//last message

        yield return new WaitUntil(() => shouldProceed);
        shouldProceed = false;

        instance.tutorialUI.CanProceed(false);
        instance.tutorialUI.gameObject.SetActive(false);//tutorial UI dissappears

        GameManager.UpdateLevelProgress(12.0f / tutorialSequenceSize, true);

        FinishTutorial();
    }
    #endregion

    private bool IsHoldingScreen()
    {
        return Input.touchCount > 0 || Input.GetMouseButton(0);
    }
}
