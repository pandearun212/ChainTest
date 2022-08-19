using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;


public class GoogleAdMobManager : MonoBehaviour
{
    private static GoogleAdMobManager _instance;
    public static GoogleAdMobManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private BannerView bannerView;
    public InterstitialAd interstitialAd;
    private string appId;
    private bool isGoogleAdInitilized;
    public bool isShowingAd;
    string bannerAdId;
    string interstitialAdId;
    void Awake()
    {
        DontDestroyOnLoad(this);
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        _instance = this;
        isShowingAd = false;
    }

    private void Start()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            /*Google Admob is initillized.*/
            isGoogleAdInitilized = true;
        });
        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1.5f);
        RequestAndLoadInterstitialAd();
    }

    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    #endregion

    #region BANNER ADS

    public void RequestBannerAd()
    {
        Debug.Log("Requesting Banner Ad.");
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        bannerAdId = adUnitId;
        // Clean up banner before reusing
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        // Add Event Handlers
        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        // Called when the paid event fired.
        this.bannerView.OnPaidEvent += HandleBannerAdPaidEvent;

        // Load a banner ad
        bannerView.LoadAd(CreateAdRequest());
    }
    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }
    private void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }
    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                           + args.ToString());
    }
    private void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }
    private void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }
    private void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
    private void HandleBannerAdPaidEvent(object sender, EventArgs e)
    {
        MonoBehaviour.print("HandleBannerAdPaidEventF event received");
    }

    #endregion

    #region INTERSTITIAL ADS

    public void RequestAndLoadInterstitialAd()
    {
        interstitialAdId = "ca-app-pub-3940256099942544/1033173712";
        // Clean up interstitial before using it
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
        interstitialAd = new InterstitialAd(interstitialAdId);

        // Add Event Handlers
        // Called when an ad request has successfully loaded.
        this.interstitialAd.OnAdLoaded += HandleOnInterstitialAdLoaded;
        // Called when an ad request failed to load.
        this.interstitialAd.OnAdFailedToLoad += HandleOnInterstitialAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitialAd.OnAdOpening += HandleOnInterstitialAdOpened;
        // Called when the ad is closed.
        this.interstitialAd.OnAdClosed += HandleOnInterstitialAdClosed;
        // Called when paid event is fired
        this.interstitialAd.OnPaidEvent += HandleInterstitialAdPaidEvent;

        // Load an interstitial ad
        interstitialAd.LoadAd(CreateAdRequest());
    }
    public void ShowInterstitialAd()
    {
        if (interstitialAd.IsLoaded())
        {
            isShowingAd = true;
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad is not ready yet");
        }
    }
    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }
    private void HandleOnInterstitialAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }
    private void HandleOnInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                           + args.ToString());
    }
    private void HandleOnInterstitialAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }
    private void HandleOnInterstitialAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        isShowingAd=false;
        DestroyInterstitialAd();
        RequestBannerAd();
        RequestAndLoadInterstitialAd();
    }
    private void HandleOnInterstitialAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }
    private void HandleInterstitialAdPaidEvent(object sender, AdValueEventArgs e)
    {
        MonoBehaviour.print("HandleInterstitialAdPaidEvent event received");
    }
    #endregion
}
