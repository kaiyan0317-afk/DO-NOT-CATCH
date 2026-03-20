using UnityEngine;
using System;
using GoogleMobileAds.Api;

public class AdMobService : MonoBehaviour
{
    public static AdMobService Instance;

    [Header("AdMob 設定")]
    // 画像から取得した本番用広告ユニットIDに書き換えました
    [SerializeField] string androidAdUnitId = "ca-app-pub-6745193529671230/7958125144";
    [SerializeField] string iosAdUnitId = "ca-app-pub-6745193529671230/7958125144";

    private string _adUnitId;
    private Action _onAdClosedAction;
    private InterstitialAd _interstitialAd;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MobileAds.Initialize(initStatus => { LoadInterstitialAd(); });
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
#if UNITY_ANDROID
       _adUnitId = androidAdUnitId;
#elif UNITY_IOS
       _adUnitId = iosAdUnitId;
#else
        // エディタ上などでテストするために、プラットフォーム外ではテストIDを使うようにしておくと便利です
        _adUnitId = "ca-app-pub-3940256099942544/1033173712";
#endif
    }

    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null) return;
            _interstitialAd = ad;
            RegisterEventHandlers(ad);
        });
    }

    public void ShowInterstitial(Action onComplete = null)
    {
        _onAdClosedAction = onComplete;

        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            AudioListener.pause = true;
            _interstitialAd.Show();
        }
        else
        {
            Debug.Log("広告準備未完了、またはプラットフォーム非対応");
            _onAdClosedAction?.Invoke();
            _onAdClosedAction = null;
            LoadInterstitialAd();
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            AudioListener.pause = false;
            _onAdClosedAction?.Invoke();
            _onAdClosedAction = null;
            LoadInterstitialAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            AudioListener.pause = false;
            _onAdClosedAction?.Invoke();
            _onAdClosedAction = null;
            LoadInterstitialAd();
        };
    }
}
