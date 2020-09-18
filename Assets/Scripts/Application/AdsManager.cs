using UnityEngine;
using GoogleMobileAds.Api;

public enum RewardType { Continue }

public class AdsManager : MonoBehaviour
{
    #region Singleton
    public static AdsManager instance { get; private set; }
    #endregion

    #region Properties
    private static bool ShowAds { get; set; }
    public static bool IsRewardedAdReady { get; set; }
    public static bool ShowingFullScreenAd { get; set; }
    #endregion

    #region Extensions
    public Purchaser purchaser;
    #endregion

    #region Ad Views
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardedAd rewardedAd;
    #endregion

    #region Events
    public UnityEngine.Events.UnityEvent OnRewardedContinue;

    private UnityEngine.Events.UnityEvent OnRewarded;
    #endregion

    private void Awake()
    {
        #region Singleton
        if (!instance)
        {
            instance = this;

            Initialize();
        }
        else if (!instance.Equals(this))
        {
            Destroy(gameObject);
        }
        #endregion
    }

    public void Initialize()
    {
        ShowAds = true;

        purchaser?.AddOnInitializedListener(
        () =>
        {
            ShowAds = !purchaser.PurchasedNoAds();
            UIManager.EnableNoAdsButton(ShowAds);
        });

        MobileAds.Initialize(
            initStatus => 
            {
                //RequestBanner();

                Debug.Log("Ads Requested.");

                RequestInterstitial();

                RequestRewarded();
            });
    }

    private void RequestBanner()
    {
        if (!ShowAds) return;

        /***REMEBER TO CHANGE AD UNIT IDS BEFORE PRODUCTION RELEASE***/
        #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
                string adUnitId = "unexpected_platform";
        #endif

        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

        AdRequest request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);

        bannerView.Show();
    }

    private void RequestInterstitial()
    {
        if (!ShowAds) return;

        #if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        #elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-3940256099942544/4411468910";
        #else
                string adUnitId = "unexpected_platform";
        #endif

        interstitial = new InterstitialAd(adUnitId);

        interstitial.OnAdOpening += HandleOnInterstitialOpening;

        interstitial.OnAdClosed += HandleOnInterstitialClosed;

        AdRequest request = new AdRequest.Builder().Build();

        interstitial.LoadAd(request);
    }

    private void RequestRewarded()
    {
        #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-3940256099942544/5224354917";
        #elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
                string adUnitId = "unexpected_platform";
        #endif

        rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;

        rewardedAd.OnAdOpening += HandleOnRewardedOpening;

        rewardedAd.OnAdClosed += HandleOnRewardedClosed;

        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

        AdRequest request = new AdRequest.Builder().Build();

        rewardedAd.LoadAd(request);
    }

    public static void ShowInterstitial()
    {
        if (!ShowAds) return;

        if (instance.interstitial?.IsLoaded() ?? false)
        {
            GameManager.HandleFullScreenAd();

            instance.interstitial.Show();
        }
    }

    public void ShowRewarded(RewardType rewardType)
    {
        #if UNITY_EDITOR
        switch (rewardType)
        {
            case RewardType.Continue:
                OnRewarded = OnRewardedContinue;
                break;
        }
        OnRewarded?.Invoke();
        #endif

        if (instance.rewardedAd?.IsLoaded() ?? false)
        {
            switch (rewardType)
            {
                case RewardType.Continue:
                    OnRewarded = OnRewardedContinue;
                    break;
            }

            instance.rewardedAd.Show();

            Debug.Log("Rewarded Ad attempting to show...");

            GameManager.HandleFullScreenAd();
        }
    }

    #region Event Handlers
    public static void HandleOnInterstitialOpening(object sender, System.EventArgs args)
    {
        instance.bannerView?.Hide();
        ShowingFullScreenAd = true;
    }

    public static void HandleOnInterstitialClosed(object sender, System.EventArgs args)
    {
        instance.bannerView?.Show();
        ShowingFullScreenAd = false;

        instance.RequestInterstitial();
    }

    public void HandleRewardedAdLoaded(object sender, System.EventArgs args)
    {
        IsRewardedAdReady = true;
    }

    public void HandleOnRewardedOpening(object sender, System.EventArgs args)
    {
        instance.bannerView?.Hide();
        ShowingFullScreenAd = true;

        Debug.Log("Showing full screen ad = " + ShowingFullScreenAd);
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);

        GameManager.HandleUserEarnedReward(OnRewarded);
    }

    public void HandleOnRewardedClosed(object sender, System.EventArgs args)
    {
        instance.bannerView?.Show();

        ShowingFullScreenAd = false;

        Debug.Log("Showing full screen ad = " + ShowingFullScreenAd);

        IsRewardedAdReady = false;
        instance.RequestRewarded();
    }
    #endregion

    private void OnApplicationQuit()
    {
        CleanUp();
    }

    public static void CleanUp()
    {
        instance.bannerView?.Destroy();
        instance.interstitial?.Destroy();
    }
}