using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using System.Threading.Tasks;

// Example for IronSource Unity.
public class AdHandler : MonoBehaviour
{
    private string ironSourceAppKey = "19e2c854d";
    public static AdHandler Instance;
    public Button rewardAdButton;
    public TextMeshProUGUI rewardText;
    public Image rewardAdImage;
    public int rewardAmount;
    public LevelManager levelManager;
    public PremiumManager premiumManager;
    public WaitForSeconds secWait = new WaitForSeconds(0.2f);
    public bool noAds;
    public GameObject rewardAd;


    public void Start()
    {
        Instance = this;
        // IronSource.setMetaData("is_deviceid_optout","true");
        // IronSource.setMetaData("is_child_directed","true");
        // IronSource.setMetaData("Google_Family_Self_Certified_SDKS","true");
        // StartCoroutine(RunIronSourceInitialise());
        RunIronSourceInitialise();
        // IronSource.Agent.loadManualRewardedVideo();
    }


    void RunIronSourceInitialise()
    {   
        IronSource.Agent.init(ironSourceAppKey);

        IronSource.Agent.validateIntegration();
        // CheckRewardsCapped();
        AttachListeners();
    }

    // void OnEnable()
    void AttachListeners()
    {
        //Add Init Event
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;

        // Add Offerwall Events
        // IronSourceEvents.onOfferwallClosedEvent += OfferwallClosedEvent;
        // IronSourceEvents.onOfferwallOpenedEvent += OfferwallOpenedEvent;
        // IronSourceEvents.onOfferwallShowFailedEvent += OfferwallShowFailedEvent;
        // IronSourceEvents.onOfferwallAdCreditedEvent += OfferwallAdCreditedEvent;
        // IronSourceEvents.onGetOfferwallCreditsFailedEvent += GetOfferwallCreditsFailedEvent;
        // IronSourceEvents.onOfferwallAvailableEvent += OfferwallAvailableEvent;

        //Add ImpressionSuccess Event
        IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;

        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

        // //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

        // //Add AdInfo Banner Events
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

        CheckRewardsCapped();
    }

    void OnApplicationPause(bool isPaused)
    {
        Debug.Log("unity-script: OnApplicationPause = " + isPaused);
        IronSource.Agent.onApplicationPause(isPaused);
    }

    public void ClickRewardedVideoButton()
    {
        Debug.Log("unity-script: ShowRewardedVideoButtonClicked");
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
        }
    }

    public void ClickOfferWallButton()
    {
        // if (IronSource.Agent.isOfferwallAvailable())
        // {
        //     IronSource.Agent.showOfferwall();
        // }
        // else
        // {
        //     Debug.Log("IronSource.Agent.isOfferwallAvailable - False");
        // }
    }

    public void StartLoadingInterstitial()
    {
        Debug.Log("unity-script: LoadInterstitialButtonClicked");
        if(noAds)
        {
            CheckRewardsCapped();
            return;
        }
        IronSource.Agent.loadInterstitial();
    }

    public void CheckRewardsCapped()
    {
        bool rewardCapped = IronSource.Agent.isRewardedVideoPlacementCapped("DefaultRewardedVideo");
        bool rewardAvailable = IronSource.Agent.isRewardedVideoAvailable();
        rewardAdImage.color = new Color(rewardAdImage.color.r, rewardAdImage.color.g, rewardAdImage.color.b, 1f);

        if(rewardCapped)
        // if(IronSource.Agent.isRewardedVideoAvailable())
        {
            rewardAdButton.interactable = false;
            rewardText.text = "No Ad";
            Debug.Log("unity-script: IronSource.Agent.isRewardAdCapped - true");
            return;
        }
        if(!rewardAvailable)
        {
            rewardAdButton.interactable = false;
            rewardText.text = "No Ad";
            Debug.Log("unity-script: IronSource.Agent.isRewardAdAvailable - false");
            return;
        }

        rewardAdButton.interactable = true;
        rewardAmount = levelManager.level * 10;
        rewardText.text = "Watch Ad +" + rewardAmount.ToString();
        Debug.Log("unity-script: IronSource.Agent.isRewardAdCapped - false");
    }

    public void StartLoadingRewardAd()
    {
        IronSource.Agent.loadRewardedVideo();
    }

    public void HideRewardAd()
    {
        Vector3 position = rewardAd.transform.localPosition;
        rewardAd.transform.localPosition = new Vector3(position.x, 500, position.z);

        // rewardAdButton.interactable = false;
        // rewardText.text = "";
        // rewardAdImage.color = new Color(rewardAdImage.color.r, rewardAdImage.color.g, rewardAdImage.color.b, 0f);
    }

    public void RevealRewardAd()
    {
        Vector3 position = rewardAd.transform.localPosition;
        rewardAd.transform.localPosition = new Vector3(position.x, 0, position.z);
    }

    public void OnLevelEnd()
    {
        // inPlay = false;
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
        if(noAds)
        {
            return;
        }
        Debug.Log("unity-script: ShowInterstitialButtonClicked");
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
        }
    }

    
    #region Init callback handlers

    void SdkInitializationCompletedEvent()
    {
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent");
    }

    #endregion

    #region AdInfo Rewarded Video
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo) {
         Debug.Log("unity-script: I got RewardedVideoOnAdOpenedEvent With AdInfo " + adInfo.ToString());
    }
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo.ToString());
        CheckRewardsCapped();
    }
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        CheckRewardsCapped();
        Debug.Log("unity-script: I got RewardedVideoOnAdAvailable With AdInfo " + adInfo.ToString());
    }
    void RewardedVideoOnAdUnavailable()
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdUnavailable");
        StartLoadingRewardAd();
    }
    void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError,IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent With Error"+ironSourceError.ToString() + "And AdInfo " + adInfo.ToString());
    }
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement,IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement.ToString()+ "And AdInfo " + adInfo.ToString());
        // int rewardAmount = ironSourcePlacement.getRewardAmount();
        Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + rewardAmount.ToString() + " name = " + ironSourcePlacement.getRewardName());
        premiumManager.UpdatePremium(rewardAmount);
    }
    void RewardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement.ToString() + "And AdInfo " + adInfo.ToString());
    }

    #endregion



    #region RewardedAd callback handlers

    void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
    {
        Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
    }

    void RewardedVideoAdOpenedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
    }

    void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
    {
        int rewardAmount = ssp.getRewardAmount();
        Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + rewardAmount.ToString() + " name = " + ssp.getRewardName());
        premiumManager.UpdatePremium(rewardAmount);
    }

    void RewardedVideoAdClosedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
    }

    void RewardedVideoAdStartedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
    }

    void RewardedVideoAdEndedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
    }

    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }

    void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
    {
        Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
    }

    /************* RewardedVideo DemandOnly Delegates *************/

    void RewardedVideoAdLoadedDemandOnlyEvent(string instanceId)
    {
        
        Debug.Log("unity-script: I got RewardedVideoAdLoadedDemandOnlyEvent for instance: " + instanceId);
    }

    void RewardedVideoAdLoadFailedDemandOnlyEvent(string instanceId, IronSourceError error)
    {
        
        Debug.Log("unity-script: I got RewardedVideoAdLoadFailedDemandOnlyEvent for instance: " + instanceId + ", code :  " + error.getCode() + ", description : " + error.getDescription());
    }

    void RewardedVideoAdOpenedDemandOnlyEvent(string instanceId)
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedDemandOnlyEvent for instance: " + instanceId);
    }

    void RewardedVideoAdRewardedDemandOnlyEvent(string instanceId)
    {
        Debug.Log("unity-script: I got RewardedVideoAdRewardedDemandOnlyEvent for instance: " + instanceId);
    }

    void RewardedVideoAdClosedDemandOnlyEvent(string instanceId)
    {
        Debug.Log("unity-script: I got RewardedVideoAdClosedDemandOnlyEvent for instance: " + instanceId);
    }

    void RewardedVideoAdShowFailedDemandOnlyEvent(string instanceId, IronSourceError error)
    {
        Debug.Log("unity-script: I got RewardedVideoAdShowFailedDemandOnlyEvent for instance: " + instanceId + ", code :  " + error.getCode() + ", description : " + error.getDescription());
    }

    void RewardedVideoAdClickedDemandOnlyEvent(string instanceId)
    {
        Debug.Log("unity-script: I got RewardedVideoAdClickedDemandOnlyEvent for instance: " + instanceId);
    }


    #endregion

    #region AdInfo Interstitial

    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got InterstitialOnAdReadyEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError) {
        Debug.Log("unity-script: I got InterstitialOnAdLoadFailed With Error " + ironSourceError.ToString());
    }

    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got InterstitialOnAdOpenedEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got InterstitialOnAdShowSucceededEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got InterstitialOnAdShowFailedEvent With Error " +ironSourceError.ToString()+ " And AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo.ToString());
        CheckRewardsCapped();
    }


    #endregion

    #region Interstitial callback handlers

    void InterstitialAdReadyEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdReadyEvent");
    }

    void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got InterstitialAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
    }

    void InterstitialAdShowSucceededEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdShowSucceededEvent");
    }

    void InterstitialAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got InterstitialAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }

    void InterstitialAdClickedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdClickedEvent");
    }

    void InterstitialAdOpenedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdOpenedEvent");
    }

    void InterstitialAdClosedEvent()
    {
        Debug.Log("unity-script: I got InterstitialAdClosedEvent");
    }

    /************* Interstitial DemandOnly Delegates *************/

    void InterstitialAdReadyDemandOnlyEvent(string instanceId)
    {
        Debug.Log("unity-script: I got InterstitialAdReadyDemandOnlyEvent for instance: " + instanceId);
    }

    void InterstitialAdLoadFailedDemandOnlyEvent(string instanceId, IronSourceError error)
    {
        Debug.Log("unity-script: I got InterstitialAdLoadFailedDemandOnlyEvent for instance: " + instanceId + ", error code: " + error.getCode() + ",error description : " + error.getDescription());
    }

    void InterstitialAdShowFailedDemandOnlyEvent(string instanceId, IronSourceError error)
    {
        Debug.Log("unity-script: I got InterstitialAdShowFailedDemandOnlyEvent for instance: " + instanceId + ", error code :  " + error.getCode() + ",error description : " + error.getDescription());
    }

    void InterstitialAdClickedDemandOnlyEvent(string instanceId)
    {
        Debug.Log("unity-script: I got InterstitialAdClickedDemandOnlyEvent for instance: " + instanceId);
    }

    void InterstitialAdOpenedDemandOnlyEvent(string instanceId)
    {
        Debug.Log("unity-script: I got InterstitialAdOpenedDemandOnlyEvent for instance: " + instanceId);
    }

    void InterstitialAdClosedDemandOnlyEvent(string instanceId)
    {
        Debug.Log("unity-script: I got InterstitialAdClosedDemandOnlyEvent for instance: " + instanceId);
    }




    #endregion

    #region Banner AdInfo

    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got BannerOnAdLoadedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError) {
        Debug.Log("unity-script: I got BannerOnAdLoadFailedEvent With Error " + ironSourceError.ToString());
    }

    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got BannerOnAdClickedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got BannerOnAdScreenPresentedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got BannerOnAdScreenDismissedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo) {
        Debug.Log("unity-script: I got BannerOnAdLeftApplicationEvent With AdInfo " + adInfo.ToString());
    }

    #endregion

    #region Banner callback handlers

    void BannerAdLoadedEvent()
    {
        Debug.Log("unity-script: I got BannerAdLoadedEvent");
    }

    void BannerAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("unity-script: I got BannerAdLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
    }

    void BannerAdClickedEvent()
    {
        Debug.Log("unity-script: I got BannerAdClickedEvent");
    }

    void BannerAdScreenPresentedEvent()
    {
        Debug.Log("unity-script: I got BannerAdScreenPresentedEvent");
    }

    void BannerAdScreenDismissedEvent()
    {
        Debug.Log("unity-script: I got BannerAdScreenDismissedEvent");
    }

    void BannerAdLeftApplicationEvent()
    {
        Debug.Log("unity-script: I got BannerAdLeftApplicationEvent");
    }

    #endregion


    #region Offerwall callback handlers

    void OfferwallOpenedEvent()
    {
        Debug.Log("I got OfferwallOpenedEvent");
    }

    void OfferwallClosedEvent()
    {
        Debug.Log("I got OfferwallClosedEvent");
    }

    void OfferwallShowFailedEvent(IronSourceError error)
    {
        Debug.Log("I got OfferwallShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }

    void OfferwallAdCreditedEvent(Dictionary<string, object> dict)
    {
        Debug.Log("I got OfferwallAdCreditedEvent, current credits = " + dict["credits"] + " totalCredits = " + dict["totalCredits"]);
        
    }

    void GetOfferwallCreditsFailedEvent(IronSourceError error)
    {
        Debug.Log("I got GetOfferwallCreditsFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }

    void OfferwallAvailableEvent(bool canShowOfferwal)
    {
        Debug.Log("I got OfferwallAvailableEvent, value = " + canShowOfferwal);
        
    }

    #endregion

    #region ImpressionSuccess callback handler

    void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
    {
        Debug.Log("unity - script: I got ImpressionSuccessEvent ToString(): " + impressionData.ToString());
        Debug.Log("unity - script: I got ImpressionSuccessEvent allData: " + impressionData.allData);
    }

    void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
    {
        Debug.Log("unity - script: I got ImpressionDataReadyEvent ToString(): " + impressionData.ToString());
        Debug.Log("unity - script: I got ImpressionDataReadyEvent allData: " + impressionData.allData);
    }

    #endregion

}
