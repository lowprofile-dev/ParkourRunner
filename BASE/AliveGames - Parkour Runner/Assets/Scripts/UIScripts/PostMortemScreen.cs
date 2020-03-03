﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ParkourRunner.Scripts.Managers;
using AEngine;

public class PostMortemScreen : MonoBehaviour
{
    public GameObject _rateMeWindow;
    public GameObject WatchAdButton;
    public Text ReviveForMoneyBtnTxt;

    public GameObject ResultsScreen;
    [SerializeField] private ResultDialogController _result;
    [SerializeField] private ReviveDialogController _revive;
    
    public GameObject NewRecordText;

    public float TimeToRevive = 5f;

    private GameManager _gm;
    private AdManager _ad;
    private bool _adSeen; //Игрок уже смотрел рекламу?
    private bool _stopTimer = false;
    private bool _isRateMeMode;

    private AudioManager _audio;

    private void Start()
    {
        _gm = GameManager.Instance;
        _ad = AdManager.Instance;
        _audio = AudioManager.Instance;

        _isRateMeMode = false;
    }

    public void Show()
    {
        _revive.Show(ReviveResultCallback);
    }

    private void ReviveResultCallback(ReviveDialogController.Results result)
    {
        switch (result)
        {
            case ReviveDialogController.Results.ShowAdvertising:
                WatchAd();
                break;

            case ReviveDialogController.Results.ReviveByCoins:
                if (_revive.IsPriceCondition && Wallet.Instance.SpendCoins(_gm.ReviveCost))
                    Revive();
                break;

            case ReviveDialogController.Results.OpenShopMenu:
                ProgressManager.SaveRecordInLeaderboards(_gm.DistanceRun);
                MenuController.TransitionTarget = MenuKinds.Shop;
                SceneManager.LoadScene("Menu");
                break;

            case ReviveDialogController.Results.TimeIsOut:
                ExitReviveScreen();
                break;
        }
    }

    public void WatchAd()
    {
        _ad.SkipAdInOrder();

        _stopTimer = true;
        _ad.ShowAdvertising(AdFinishedCallback, AdSkippedCallback, AdSkippedCallback);
    }

    private void AdFinishedCallback()
    {
        print("Ad is ended. Revive...");
        //_adSeen = true;
        Revive();
        _stopTimer = false;
    }

    private void AdSkippedCallback()
    {
        _stopTimer = false;
        print("Skipped or cancelled revive AD");
    }
        
    public void CheckRateMe()
    {
        EnvironmentController.CheckKeys();
        if (!PlayerPrefs.HasKey("WAS_RATE"))
        {
            PlayerPrefs.SetInt("WAS_RATE", 0);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.GetInt(EnvironmentController.TUTORIAL_KEY) != 1 && PlayerPrefs.GetInt(EnvironmentController.ENDLESS_KEY) != 1 && PlayerPrefs.GetInt("WAS_RATE") != 1)
        {
            if (PlayerPrefs.GetInt(EnvironmentController.LEVEL_KEY) == 3)
            {
                PlayerPrefs.SetInt("WAS_RATE", 1);
                ShowRateMe();

                _audio.PlaySound(Sounds.WinQuest);
            }
            else
                ExitReviveScreen();

            PlayerPrefs.Save();
        }
        else
            ExitReviveScreen();
    }

    public void Revive()
    {
        ResultsScreen.SetActive(false);
        _revive.Hide();
        _gm.Revive();
    }

    public void ShowRateMe()
    {
        _revive.Hide();
        ResultsScreen.SetActive(false);

        _rateMeWindow.SetActive(true);
    }

    public void ExitReviveScreen()
    {
        Wallet.Instance.Save();

        _revive.Hide();
        _result.Show();
        
        _audio.PlaySound(Sounds.ResultFull);
                
        NewRecordText.SetActive(ProgressManager.IsNewRecord(_gm.DistanceRun));
                
        if (_ad.CheckAdvertisingOrder())
            _ad.ShowAdvertising(null, null, null);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus && _isRateMeMode)
        {
            _isRateMeMode = false;
            _rateMeWindow.SetActive(false);
            ExitReviveScreen();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && _isRateMeMode)
        {
            _isRateMeMode = false;
            _rateMeWindow.SetActive(false);
            ExitReviveScreen();
        }
    }

    public void OnRateButtonClick()
    {
        AudioManager.Instance.PlaySound(Sounds.Tap);

        _isRateMeMode = true;
        GiveLike.OpenStore();
    }

    public void OnSkipRateButtonClick()
    {
        AudioManager.Instance.PlaySound(Sounds.Tap);

        _rateMeWindow.SetActive(false);
        ExitReviveScreen();
    }
}