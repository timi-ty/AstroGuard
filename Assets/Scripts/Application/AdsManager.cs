﻿using UnityEngine;
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

        //RequestConfiguration.Builder builder = new RequestConfiguration.Builder();
        //builder.SetTestDeviceIds(new System.Collections.Generic.List<string> { "E1B7E7E9AB7C336509E2FCA9880E8FB4" });
        //MobileAds.SetRequestConfiguration(builder.build());

        MobileAds.Initialize(
            initStatus => 
            {
                Debug.Log("Ads Requested.");

                RequestInterstitial();

                RequestRewarded();
            });
    }

    private void RequestInterstitial()
    {
        if (!ShowAds) return;

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7435320572534602/7085498581";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-7435320572534602/2566834925";
#else
        string adUnitId = "unexpected_platform";
#endif

        interstitial = new InterstitialAd(adUnitId);

        interstitial.OnAdOpening += HandleOnInterstitialOpening;

        interstitial.OnAdClosed += HandleOnInterstitialClosed;

        interstitial.OnAdFailedToLoad += HandleAdFailedToLoad;

        AdRequest request = new AdRequest.Builder().Build();

        interstitial.LoadAd(request);
    }

    private void RequestRewarded()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-7435320572534602/7476380825";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-7435320572534602/1586771659";
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
        if (!ShowAds || GameManager.instance.disableInterstitialAds) return;

        if (instance.interstitial?.IsLoaded() ?? false)
        {
            GameManager.HandleFullScreenAd();

            instance.interstitial.Show();
        }
    }

    public void ShowRewarded(RewardType rewardType)
    {
        if (GameManager.instance.rewardsWithoutAds)
        {
            switch (rewardType)
            {
                case RewardType.Continue:
                    OnRewarded = OnRewardedContinue;
                    break;
            }
            OnRewarded?.Invoke();

            return;
        }

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

        IsRewardedAdReady = false;
        instance.RequestRewarded();
    }

    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Ad Failed: " + args.Message);
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