//In Progress
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager instance { get; private set; }
    #endregion

    #region Constants
    private const int RETRY_COUNT_DOWN_DURATION = 5;
    #endregion

    #region UI Elements
    [Header("Major UI Screens")]
    public GameObject homeScreen;
    public SettingsPopup settingsPopup;
    public PlayHUD playHud;
    public GameObject retryScreen;
    public LevelsUI levelsScreen;
    public LeaderboardUI leaderboardScreen;
    public ShopUI shopScreen;
    public GameObject creditsScreen;

    [Header("Mutable UI Elemets")]
    public TextMeshProUGUI sessionScoreText;
    public TextMeshProUGUI greetingsText;
    public ExperienceBar experienceBar;
    public ProgressBar retryCountdownBar;
    public AstroGoldDisplay astroGoldDisplay;
    public Button watchAdContinueButton;
    public Button noAdsButton;
    public Button facebookButton;
    public Button leaderBoardButton;
    public TransitionPanel transitionPanel;

    [Header("Application UI Elements")]
    public AlertDialog alertDialog;
    #endregion

    #region Worker Parameters
    private Coroutine retryCountdownCoroutine;
    private bool mainUIDirty;
    #endregion

    #region Unity Runtime
    private void Awake()
    {
        #region Singleton
        DontDestroyOnLoad(gameObject);

        if (!instance)
        {
            instance = this;

            FirebaseUtility.AddOnAuthListener(
            currentUser => 
            {
                mainUIDirty = true;
            });

            StartCoroutine(RefreshMainUICoroutine());
        }
        else if (!instance.Equals(this))
        {
            instance.GetComponentInChildren<Canvas>().worldCamera = Camera.main;
            Destroy(gameObject);
        }
        #endregion
    }
    #endregion

    #region Game UI
    public static void ShowMainUI()
    {
        HideAllUIScreens();
        instance.homeScreen.SetActive(true);
        instance.experienceBar.Refresh();
        instance.astroGoldDisplay.Refresh();
    }

    private IEnumerator RefreshMainUICoroutine()
    {
        while (true)
        {
            yield return new WaitUntil(() => mainUIDirty);
            RefreshGreetings(FirebaseUtility.CurrentUser?.DisplayName);
            EnableFacebookButton(FirebaseUtility.CurrentUser == null);
            EnableLeaderboardButton(FirebaseUtility.CurrentUser != null);
            mainUIDirty = false;
            yield return new WaitForSecondsRealtime(2.0f);
        }
    }

    public static void ShowPlayUI()
    {
        HideAllUIScreens();
        instance.playHud.Toggle(true);
    }

    public static void ShowPauseUI()
    {
        HideAllUIScreens();
        instance.playHud.Toggle(false);
    }

    public static void ShowRetryUI()
    {
        HideAllUIScreens();
        instance.retryScreen.SetActive(true);

        EnableContinueButton(AdsManager.IsRewardedAdReady);

        #if UNITY_EDITOR
        EnableContinueButton(true);
        #endif

        instance.sessionScoreText.text = Session.Score.ToString();

        instance.retryCountdownCoroutine = instance.StartCoroutine(instance.CountDownToRetry());
    }

    public void ShowSettings()
    {
        instance.settingsPopup.Show();
    }

    public void ShowLevelsUI()
    {
        Transition(
            () =>
            {
                HideAllUIScreens();
                levelsScreen.Show();
            },
            showInterstitial: false);
    }

    public void ShowLeaderboardUI()
    {
        Transition(
            () =>
            {
                HideAllUIScreens();
                leaderboardScreen.Show();
            },
            showInterstitial: false);
    }

    public void ShowStoreUI()
    {
        Transition(
            () =>
            {
                HideAllUIScreens();
                shopScreen.Show();
            },
            showInterstitial: false);
    }

    public static void ShowCredits()
    {
        Transition(
            () =>
            {
                HideAllUIScreens();
                instance.creditsScreen.SetActive(true);
            },
            showInterstitial: false);
    }

    public static void UpdateHUD()
    {
        instance.playHud.UpdateHud(immediately: false);
    }
    #endregion

    #region Application UI
    public static void RefreshGreetings(string userName)
    {
        if (userName != null)
        {
            instance.greetingsText.text = "HELLO, " + userName.ToUpper();
        }
        else
        {
            instance.greetingsText.text = "HELLO...";
        }
    }

    public static void EnableContinueButton(bool enable)
    {
        Animator floatAnim = instance.watchAdContinueButton.GetComponent<Animator>();
        instance.watchAdContinueButton.interactable = enable;
        floatAnim.enabled = enable;
    }

    public static void EnableNoAdsButton(bool enable)
    {
        Animator dancerAnim = instance.noAdsButton.GetComponent<Animator>();
        instance.noAdsButton.interactable = enable;
        dancerAnim.enabled = enable;
    }

    public static void EnableFacebookButton(bool enable)
    {
        Animator dancerAnim = instance.facebookButton.GetComponent<Animator>();
        instance.facebookButton.interactable = enable;
        dancerAnim.enabled = enable;
    }

    public static void EnableLeaderboardButton(bool enable)
    {
        instance.leaderBoardButton.interactable = enable;
    }

    public static void QueueAlertDialog(AlertMessageInfo alertMessageInfo, UnityAction positiveAction, UnityAction negativeAction, string alertId)
    {
        instance.alertDialog.Queue(alertMessageInfo, positiveAction, negativeAction, alertId);
    }
    #endregion

    #region Utility Methods
    private static void HideAllUIScreens()
    {
        instance.homeScreen.SetActive(false);
        instance.playHud.Hide();
        instance.retryScreen.SetActive(false);
        instance.levelsScreen.Hide();
        instance.leaderboardScreen.Hide();
        instance.shopScreen.Hide();
        instance.creditsScreen.SetActive(false);
    }

    public static void Notify(Objective objective)
    {
        instance.playHud.Notify(objective);
    }

    public static void Transition(UnityAction transitionCallback, string title, string subTitle, bool showInterstitial)
    {
        instance.StartCoroutine(instance.TransitionCoroutine(transitionCallback, title, subTitle, true, showInterstitial));
    }

    public static void Transition(UnityAction transitionCallback, bool showInterstitial)
    {
        instance.StartCoroutine(instance.TransitionCoroutine(transitionCallback, "", "", false, showInterstitial));
    }

    public IEnumerator TransitionCoroutine(UnityAction transitionCallback, string title, string subTitle, bool wait, bool showInterstitial)
    {
        transitionPanel.StartTransition(title, subTitle, wait);

        float progress = 0;

        while (progress < 1)
        {
            progress = Mathf.Lerp(progress, 1.2f, Time.unscaledDeltaTime * 10);
            transitionPanel.transitionProgress = progress;
            yield return null;
        }

        GameManager.PauseGame();

        if (wait)
        {
            yield return new WaitForSecondsRealtime(3.0f);
        }

        GameManager.ResumeGame();

        #region Interstitial Ad
        if (showInterstitial)
        {
            Debug.Log("Showing Interstitial...");
            AdsManager.ShowInterstitial();

            yield return new WaitWhile(() => GameManager.gameFrozen);
        }
        #endregion

        transitionCallback?.Invoke();

        while (progress > 0)
        {
            progress = Mathf.Lerp(progress, -0.2f, Time.unscaledDeltaTime * 8);
            transitionPanel.transitionProgress = progress;
            yield return null;
        }

        transitionPanel.FinishTransition();
    }
    #endregion

    #region Control Methods
    private IEnumerator CountDownToRetry()
    {
        retryCountdownBar.SetProgressImmediate(1, RETRY_COUNT_DOWN_DURATION.ToString());

        Animator pulseAnim = retryCountdownBar.GetComponent<Animator>();
        int pulseAnimHash = Animator.StringToHash("pulseCount");

        float remainingTime = RETRY_COUNT_DOWN_DURATION;

        while (remainingTime > 0)
        {
            if (retryCountdownBar.gameObject.activeInHierarchy && !GameManager.gameFrozen)
            {
                remainingTime -= Time.unscaledDeltaTime;

                float progress = remainingTime / RETRY_COUNT_DOWN_DURATION;

                retryCountdownBar.SetProgressImmediate(progress, ((int) remainingTime).ToString());

                pulseAnim.SetTrigger(pulseAnimHash);

            }
            yield return null;
        }
        GameManager.instance.OnRetry();
    }

    public void AbortRetryCountdown()
    {
        if (retryCountdownCoroutine != null)
        {
            StopCoroutine(retryCountdownCoroutine);
        }
    }
    #endregion
}
