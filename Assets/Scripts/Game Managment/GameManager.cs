//In Progress
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance { get; private set; }
    #endregion

    #region Developer Settings
    [Header("Developer Settings")]
    [Tooltip("DEVELOPER ONLY! Disable for production build.")]
    public bool unlockAllLevels;
    [Tooltip("DEVELOPER ONLY! Disable for production build.")]
    public bool disableInterstitialAds;
    [Tooltip("DEVELOPER ONLY! Disable for production build.")]
    public bool rewardsWithoutAds;
    #endregion

    #region Basic Settings
    private static float _scale;
    public static float universalGameScale
    {
        private set => _scale = value;
        get
        {
            return _scale / 0.7320234f;
        }
    }
    public static float defaultTimeScale => 1.0f;
    #endregion

    #region Global Static Parameters
    public static int currentLevel => LevelManager.CurrentLevel;
    public static float levelProgress { get; set; }
    public static bool isInGame => currentLevel != 0;
    public static bool gameFrozen { get; set; }
    #endregion

    #region Game Components
    public Background background { get; set; }
    public PlayerBehaviour player { get; set; }
    public Ship ship { get; set; }
    public AsteroidCommander asteroidCoordinator { get; set; }
    public SpawnerManager spawnerCoordinator { get; set; }
    public ItemShop shop { get; set; }
    #endregion

    #region Worker Parameters
    private float _cachedTimeScale { get; set; }
    #endregion

    #region Unity Runtime
    private void Awake()
    {
        #region Singleton
        DontDestroyOnLoad(gameObject);

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

    #region Scene Loading
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        InitializeGame();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Events
    private UnityEvent activeRewardEvent;
    #endregion

    #region Game State Management
    private void InitializeGame()
    {
        FindGameComponents();

        ScaleGameToScreenScreen();

        Time.timeScale = defaultTimeScale;


        player.OnInitialize();

        if (currentLevel <= 0)
        {
            UIManager.ShowMainUI();
        }
        else if (currentLevel > 0)
        {
            OnPlay(currentLevel);
        }
    }

    public void GoHome()
    {
        LevelManager.Home();

        AudioManager.TurnUpBgMusic();

        UIManager.instance.AbortRetryCountdown();

        UIManager.Transition(
            () =>
            {
                FirebaseUtility.SyncGameData();
                SceneManager.LoadScene(0);
            },
            showInterstitial: false);

        Session.Instance.Suspend();
    }

    public void OnPlay(int level)
    {
        if (!LevelManager.IsLevelValid(level)) return;

        LevelInfo levelInfo = LevelManager.GetLevelInfo(level);

        PlayLevel(levelInfo);
    }

    public void OnPlay()
    {
        LevelInfo levelInfo = LevelManager.GetLevelInfo();

        PlayLevel(levelInfo);
    }

    private void PlayLevel(LevelInfo levelInfo)
    {
        Session.Instance.Resume();

        AudioManager.TurnDownBgMusic();

        ObjectiveManager.Refresh();

        ObjectiveManager.SaveActiveToFirebase();

        levelProgress = 0;

        Time.timeScale = defaultTimeScale;

        UIManager.ShowPlayUI();

        background.OnPlay(levelInfo.environmentSettings.backgroundAssetIndex);
        player.OnPlay();
        ship.OnPlay();
        asteroidCoordinator.OnPlay(levelInfo.enemyLineup);
        spawnerCoordinator.OnPlay(levelInfo.objectSpawnSettings);
    }

    public void OnPause()
    {
        PauseGame();

        UIManager.ShowPauseUI();

        player.OnPause();
    }

    public void OnResume()
    {
        ResumeGame();

        UIManager.ShowPlayUI();

        player.OnResume();
    }

    public void OnFailed()
    {
        player.Die();

        UIManager.ShowRetryUI();

        Session.Instance.Reset();
    }

    public void OnContinue()
    {
        Session.Instance.Restore();

        UIManager.instance.AbortRetryCountdown();

        UIManager.ShowPlayUI();

        ResumeGame();

        player.OnContinue();
        ship.OnContinue();
    }

    public void OnRetry()
    {
        UIManager.instance.AbortRetryCountdown();

        bool showInterstitial = Random.Range(0, 2) == 0;

        UIManager.Transition(
            () =>
            {
                SceneManager.LoadScene(0);
            },
            "Let's Try That Again", "Level " + currentLevel.ToString("D2"),
            showInterstitial: showInterstitial);
    }

    public void OnLevelFinished()
    {
        if (Session.Instance.ShipHealth <= 0) return;

        Session.Instance.Bind();

        AudioManager.TurnUpBgMusic();

        if (LevelManager.IsLastLevel(currentLevel))
        {
            OnGameFinished();
        }
        else
        {
            LevelManager.NextLevel();

            bool showInterstitial = Random.Range(0, 2) == 0;

            UIManager.Transition(
            () =>
            {
                if (currentLevel % 3 == 0)
                    FirebaseUtility.SyncGameData();

                SceneManager.LoadScene(0);
            },
            "Next Level!", currentLevel.ToString("D2"),
            showInterstitial: showInterstitial);
        }
    }

    public void OnGameFinished()
    {
        UIManager.ShowCredits();
        FirebaseUtility.SyncGameData();
    }
    #endregion

    #region Game Operations
    public static void SpawnGoldCoin()
    {
        instance.spawnerCoordinator.SpawnGoldCoin();
    }

    public static void SpawnGoldCoin(Vector2 position)
    {
        instance.spawnerCoordinator.SpawnGoldCoin(position);
    }

    public static void ActivateContent(int itemIndex)
    {
        Item item = instance.shop.Items[itemIndex];

        switch (item.itemType)
        {
            case ItemType.Core:
                instance.player.ChangeCore(item.itemResource, item.itemColor);
                PlayerStats.Instance.activeCoreIndex = itemIndex;
                break;
            case ItemType.Blade:
                instance.player.ChangeBlade(item.itemResource, item.itemColor);
                PlayerStats.Instance.activeBladeIndex = itemIndex;
                break;
        }
    }

    public static void UpdateLevelProgress(float progress)
    {
        levelProgress = progress;

        UIManager.UpdateHUD();
    }
    #endregion

    #region Utility Methods
    private void ScaleGameToScreenScreen()
    {
        float playerWidth = player.GetComponent<SpriteRenderer>().bounds.size.x;

        float recommendedPlayerWidth = ScreenBounds.width / 16.0f;

        float resultingScale = recommendedPlayerWidth / playerWidth;

        player.SetScale(resultingScale);

        universalGameScale = resultingScale;
    }

    private void FindGameComponents()
    {
        background = FindObjectOfType<Background>();
        player = FindObjectOfType<PlayerBehaviour>();
        ship = FindObjectOfType<Ship>();
        asteroidCoordinator = FindObjectOfType<AsteroidCommander>();
        spawnerCoordinator = FindObjectOfType<SpawnerManager>();
        shop = FindObjectOfType<ItemShop>();
    }

    public static void PauseGame()
    {
        if (gameFrozen)
        {
            Debug.LogWarning("Game cannot be paused while it is frozen.");
            return;
        }
        if (Time.timeScale == 0)
        {
            Debug.Log("Pause called but game was already paused.");
            return;
        }
        instance._cachedTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public static void ResumeGame()
    {
        if (gameFrozen)
        {
            Debug.LogWarning("Game cannot be resumed while it is frozen.");
            return;
        }
        if (Time.timeScale != 0)
        {
            Debug.Log("Resume called but game was already playing.");
            return;
        }
        Time.timeScale = instance._cachedTimeScale;
    }

    public static void HandleFullScreenAd()
    {
        instance.StartCoroutine(instance.FullScreenAdCoroutine());
    }

    public static void HandleUserEarnedReward(UnityEvent rewardEvent)
    {
        instance.activeRewardEvent = rewardEvent;
    }

    private IEnumerator FullScreenAdCoroutine()
    {
        FreezeGame();

        yield return new WaitForSecondsRealtime(2.0f);

        while (AdsManager.ShowingFullScreenAd)
        {
            Debug.Log("GM: Showing full screen ad = " + AdsManager.ShowingFullScreenAd);
            yield return new WaitForSecondsRealtime(1.0f);
        }
        //yield return new WaitWhile(() => AdsManager.ShowingFullScreenAd);
        //Debug.Log("Waited While");
        UnfreezeGame();
        activeRewardEvent?.Invoke();
        activeRewardEvent = null;
    }

    public static void FreezeGame()
    {
        if (gameFrozen)
        {
            Debug.Log("Freeze called but game was already frozen.");
            return;
        }
        Debug.Log("Freezing...");
        PauseGame();
        AudioManager.FreezeAudio();
        gameFrozen = true;
        return;
    }

    public static void UnfreezeGame()
    {
        if (!gameFrozen)
        {
            Debug.Log("Unfreeze called but game was not frozen.");
            return;
        }
        Debug.Log("Unfreezing...");
        gameFrozen = false;
        ResumeGame();
        AudioManager.UnfreezeAudio();
        return;
    }

    public static bool IsPlayerAlive()
    {
        return isInGame && instance.player.gameObject.activeInHierarchy;
    }
    #endregion
}
