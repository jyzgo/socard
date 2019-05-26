#define ADMOB_ENABLED
using UnityEngine;
using System.Collections;
using MTUnity;
using GoogleMobileAds.Api;

public static class AdMgr  {
    const string APP_ID = "ca-app-pub-6770182166257156~1123700634";
    const string AND_BANNER = "ca-app-pub-6770182166257156/9205779753";
    const string AND_INTER = "ca-app-pub-6770182166257156/6771188102";

    //const string TEST_APP_ID = "ca-app-pub-6770182166257156~8934317012";
    //const string TEST_AND_BANNER = "ca-app-pub-6770182166257156/2999464050";
    //const string TEST_AND_INTER = "ca-app-pub-6770182166257156/7852760510";

    

    const string APPNILE_AND_BANNER = "";
    const string APPNILE_AND_INTER = "";
    const string APPNILE_AND_NATIVE_BAN = "";
#if UNITY_IOS
    const string BANNER = APPNILE_IOS_BANNER;
    const string INTER = APPNILE_IOS_INTER;
    const string NATIVE = APPNILE_IOS_NATIVE_BAN;
#endif


    public static void RegisterAllAd(LevelMgr lvMgr)
    {
        MobileAds.Initialize(APP_ID);
        RequestBanner();
    }

    private static BannerView bannerView;
    private static void RequestBanner()
    {
        string adUnitId = AND_BANNER;

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        AdRequest request = new AdRequest.Builder().Build();

        bannerView.LoadAd(request);
        
    }


    //    static void RegiseterAdmob(LevelMgr lvMgr)
    //    {
    //        ad = Admob.Instance();
    //        ad.bannerEventHandler += lvMgr.onBannerEvent;
    //        //ad.setTesting(true);
    //        ad.interstitialEventHandler += lvMgr.onInterstitialEvent;
    //        ad.rewardedVideoEventHandler += lvMgr.onRewardedVideoEvent;
    //        ad.nativeBannerEventHandler += lvMgr.onNativeBannerEvent;
    //        ad.initAdmob("", INTER);

    //        //   ad.setTesting(true);
    //        //ad.setGender(AdmobGender.MALE);
    //        //string[] keywords = { "game", "crash", "male game" };
    //        //ad.setKeywords(keywords);

    //        PreloadAdmobInterstitial();

    //        //PreloadAdmobRewaredVideo();
    //        //Debug.Log("admob inited -------------");
    //    }
    //#if (ADMOB_ENABLED)
    //    static Admob ad;
    //#endif

    public static void ShowAdmobInterstitial()
    {
        _interstitial.Show();

    }

    public static void PreloadAdmobInterstitial()
    {
        if (_interstitial == null)
        {
            RequestInterstitial();
        }
      
            AdRequest request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            _interstitial.LoadAd(request);
    }



    //    public static void PreloadAdmobRewaredVideo()
    //    {
    //#if (ADMOB_ENABLED)
    //        if (!ad.isRewardedVideoReady())
    //        {
    //            ad.loadRewardedVideo(ADMOB_REWARDVIDEO_ID);
    //        }
    //#endif
    //    }

    //    const string ADMOB_REWARDVIDEO_ID = "ca-app-pub-8151883983314364/7129319338";
    //    public static void ShowAdmobRewardVideo()
    //    {
    //#if (ADMOB_ENABLED)
    //        if (ad.isRewardedVideoReady())
    //        {
    //            ad.showRewardedVideo();
    //        }
    //        else
    //        {
    //            ad.loadRewardedVideo(ADMOB_REWARDVIDEO_ID);

    //        }
    //#endif
    //    }

    public static void ShowAdmobBanner()
    {
        bannerView.Show();
    }

    private static InterstitialAd _interstitial;

    private static void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = AND_INTER;
#endif

        _interstitial = new InterstitialAd(adUnitId);
   
    }

    public static bool IsAdmobInterstitialReady()
    {
        if (_interstitial == null)
        {
            return false;
        }
        else
        {
            return _interstitial.IsLoaded();
        }
    }

    //    public static void ShowNativeBanner(int w,int h,int x,int y )
    //    {
    //        return;
    //#if (ADMOB_ENABLED)
    //        if (ad != null)
    //        {
    //           //ad.showNativeBannerAbsolute(new AdSize(w, h), x, y, OMG_NATIVE_BANNER_ID);
    //            //ad.showNativeBannerRelative(new AdSize(w, h), AdPosition.TOP_CENTER, y, NATIVE);
    //        }

    //#endif

    //    }


    //    public static void HideNativeBanner()
    //    {
    //#if (ADMOB_ENABLED)

    //        ad.removeNativeBanner();
    //#endif
    //    }



    //#endregion Admob


    //    public static void RegisterAllAd(LevelMgr lv)
    //    {
    //#if (ADMOB_ENABLED)
    //        RegiseterAdmob(lv);
    //#endif

    //    }






}
