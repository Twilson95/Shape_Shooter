using System;
using TMPro;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.UI;

public class AdHandler : MonoBehaviour
{
    [SerializeField] private string appKey = "19e2c854d";
    [SerializeField] private string rewardedAdUnitId = "";
    [SerializeField] private string interstitialAdUnitId = "";
    [SerializeField] private string rewardedPlacementName = "DefaultRewardedVideo";

    public static AdHandler Instance;
    public Button rewardAdButton;
    public TextMeshProUGUI rewardText;
    public Image rewardAdImage;
    public int rewardAmount;
    public LevelManager levelManager;
    public PremiumManager premiumManager;
    public bool noAds;
    public GameObject rewardAd;

    private LevelPlayRewardedAd rewardedAd;
    private LevelPlayInterstitialAd interstitialAd;

    public void Start()
    {
        Instance = this;
        InitializeLevelPlay();
    }

    private void InitializeLevelPlay()
    {
        LevelPlay.OnInitSuccess += OnSdkInitializationCompleted;
        LevelPlay.OnInitFailed += OnSdkInitializationFailed;
        LevelPlay.Init(appKey);
    }

    private void OnSdkInitializationCompleted(LevelPlayConfiguration configuration)
    {
        Debug.Log("unity-script: LevelPlay SDK initialization completed");

        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);
        rewardedAd.OnAdLoaded += RewardedOnAdLoadedEvent;
        rewardedAd.OnAdLoadFailed += RewardedOnAdLoadFailedEvent;
        rewardedAd.OnAdDisplayed += RewardedOnAdDisplayedEvent;
        rewardedAd.OnAdDisplayFailed += RewardedOnAdDisplayFailedEvent;
        rewardedAd.OnAdRewarded += RewardedOnAdRewardedEvent;
        rewardedAd.OnAdClosed += RewardedOnAdClosedEvent;
        rewardedAd.OnAdClicked += RewardedOnAdClickedEvent;

        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);
        interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;

        StartLoadingInterstitial();
        StartLoadingRewardAd();
        CheckRewardsCapped();
    }

    private void OnSdkInitializationFailed(LevelPlayInitError error)
    {
        Debug.LogError("unity-script: LevelPlay initialization failed: " + error);
        CheckRewardsCapped();
    }

    private void OnDestroy()
    {
        LevelPlay.OnInitSuccess -= OnSdkInitializationCompleted;
        LevelPlay.OnInitFailed -= OnSdkInitializationFailed;

        if (rewardedAd != null)
        {
            rewardedAd.OnAdLoaded -= RewardedOnAdLoadedEvent;
            rewardedAd.OnAdLoadFailed -= RewardedOnAdLoadFailedEvent;
            rewardedAd.OnAdDisplayed -= RewardedOnAdDisplayedEvent;
            rewardedAd.OnAdDisplayFailed -= RewardedOnAdDisplayFailedEvent;
            rewardedAd.OnAdRewarded -= RewardedOnAdRewardedEvent;
            rewardedAd.OnAdClosed -= RewardedOnAdClosedEvent;
            rewardedAd.OnAdClicked -= RewardedOnAdClickedEvent;
        }

        if (interstitialAd != null)
        {
            interstitialAd.OnAdLoaded -= InterstitialOnAdLoadedEvent;
            interstitialAd.OnAdLoadFailed -= InterstitialOnAdLoadFailedEvent;
            interstitialAd.OnAdDisplayed -= InterstitialOnAdDisplayedEvent;
            interstitialAd.OnAdDisplayFailed -= InterstitialOnAdDisplayFailedEvent;
            interstitialAd.OnAdClicked -= InterstitialOnAdClickedEvent;
            interstitialAd.OnAdClosed -= InterstitialOnAdClosedEvent;
        }
    }

    public void ClickRewardedVideoButton()
    {
        Debug.Log("unity-script: ShowRewardedVideoButtonClicked");

        if (rewardedAd != null && rewardedAd.IsAdReady() && !LevelPlayRewardedAd.IsPlacementCapped(rewardedPlacementName))
        {
            rewardedAd.ShowAd(rewardedPlacementName);
        }
        else
        {
            Debug.Log("unity-script: rewarded ad not ready or placement capped");
        }
    }

    public void StartLoadingInterstitial()
    {
        if (noAds || interstitialAd == null)
        {
            CheckRewardsCapped();
            return;
        }

        Debug.Log("unity-script: LoadInterstitialButtonClicked");
        interstitialAd.LoadAd();
    }

    public void CheckRewardsCapped()
    {
        rewardAdImage.color = new Color(rewardAdImage.color.r, rewardAdImage.color.g, rewardAdImage.color.b, 1f);

        bool rewardCapped = LevelPlayRewardedAd.IsPlacementCapped(rewardedPlacementName);
        bool rewardAvailable = rewardedAd != null && rewardedAd.IsAdReady();

        if (rewardCapped || !rewardAvailable)
        {
            rewardAdButton.interactable = false;
            rewardText.text = "No Ad";
            return;
        }

        rewardAdButton.interactable = true;
        rewardAmount = levelManager.level * 10;
        rewardText.text = "Watch Ad +" + rewardAmount;
    }

    public void StartLoadingRewardAd()
    {
        if (rewardedAd == null)
        {
            return;
        }

        rewardedAd.LoadAd();
    }

    public void HideRewardAd()
    {
        Vector3 position = rewardAd.transform.localPosition;
        rewardAd.transform.localPosition = new Vector3(position.x, 500, position.z);
    }

    public void RevealRewardAd()
    {
        Vector3 position = rewardAd.transform.localPosition;
        rewardAd.transform.localPosition = new Vector3(position.x, 0, position.z);
    }

    public void OnLevelEnd()
    {
        ShowInterstital();
        StartLoadingRewardAd();
        RevealRewardAd();
    }

    public void OnLevelStart()
    {
        StartLoadingInterstitial();
        StartLoadingRewardAd();
        HideRewardAd();
    }

    public void ShowInterstital()
    {
        if (noAds)
        {
            return;
        }

        Debug.Log("unity-script: ShowInterstitialButtonClicked");
        if (interstitialAd != null && interstitialAd.IsAdReady())
        {
            interstitialAd.ShowAd();
        }
        else
        {
            Debug.Log("unity-script: interstitial not ready");
        }
    }

    private void RewardedOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: rewarded ad loaded: " + adInfo);
        CheckRewardsCapped();
    }

    private void RewardedOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log("unity-script: rewarded ad load failed: " + error);
        CheckRewardsCapped();
    }

    private void RewardedOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: rewarded ad displayed: " + adInfo);
    }

    private void RewardedOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.Log("unity-script: rewarded ad display failed: " + error + " | " + adInfo);
        CheckRewardsCapped();
    }

    private void RewardedOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward adReward)
    {
        Debug.Log("unity-script: rewarded user. adInfo=" + adInfo + " reward=" + adReward);

        int payout = rewardAmount;
        if (adReward != null && adReward.Amount > 0)
        {
            payout = adReward.Amount;
        }

        premiumManager.UpdatePremium(payout);
    }

    private void RewardedOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: rewarded ad closed: " + adInfo);
        StartLoadingRewardAd();
        CheckRewardsCapped();
    }

    private void RewardedOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: rewarded ad clicked: " + adInfo);
    }

    private void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: interstitial loaded: " + adInfo);
    }

    private void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        Debug.Log("unity-script: interstitial load failed: " + error);
    }

    private void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: interstitial displayed: " + adInfo);
    }

    private void InterstitialOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        Debug.Log("unity-script: interstitial display failed: " + error + " | " + adInfo);
    }

    private void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: interstitial clicked: " + adInfo);
    }

    private void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: interstitial closed: " + adInfo);
        StartLoadingInterstitial();
        CheckRewardsCapped();
    }
}
