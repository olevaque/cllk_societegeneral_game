using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppGame : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private CaptainController captainController;

    [Header("ChooseGameVersion")]
    [SerializeField] private GameObject chooseVersionPnl, gamePnl, leftPartPnl;
    [SerializeField] private Button playVersionA, playVersionB;

    private void Awake()
    {
        chooseVersionPnl.SetActive(true);
        gamePnl.SetActive(false);
    }

    private void OnEnable()
    {
        captainController.onProposePassword += OnProposePassword;
        captainController.onProposeCode += OnProposeCode;
        captainController.onProposeSendReport += OnProposeSendReport;
        captainController.onProposeFinalChoose += OnProposeFinalChoose;

        playVersionA.onClick.AddListener(OnVersionAClick);
        playVersionB.onClick.AddListener(OnVersionBClick);

        Main.TimerManager.OnTimerComplete.AddListener(OnTimerComplete);
        Main.TimerManager.OnTimerComplete.AddListener(OnTimerUpdate);
    }

    private void OnDisable()
    {
        captainController.onProposePassword -= OnProposePassword;
        captainController.onProposeCode -= OnProposeCode;
        captainController.onProposeSendReport -= OnProposeSendReport;
        captainController.onProposeFinalChoose -= OnProposeFinalChoose;

        playVersionA.onClick.RemoveListener(OnVersionAClick);
        playVersionB.onClick.RemoveListener(OnVersionBClick);

        Main.TimerManager.OnTimerComplete.RemoveListener(OnTimerComplete);
        Main.TimerManager.OnTimerComplete.RemoveListener(OnTimerUpdate);
    }

    private void Start()
    {
        captainController.GotoStep(CaptainController.STEP.PASSWORD);
    }

    private void OnVersionAClick()
    {
        GameVersion.IsVersionA = true;

        StartGame();
    }

    private void OnVersionBClick()
    {
        GameVersion.IsVersionA = false;

        StartGame();
    }
    
    private void StartGame()
    {
        captainController.Initialise();

        chooseVersionPnl.SetActive(false);
        gamePnl.SetActive(true);
    }

    private void OnTimerUpdate()
    {
        throw new NotImplementedException();
    }

    private void OnTimerComplete()
    {
        throw new NotImplementedException();
    }

    private void OnProposePassword(string pass)
    {
        if ((GameVersion.IsVersionA && pass == "Y5rd6C12m") || (!GameVersion.IsVersionA && pass == "Z6se7D23n"))
        {
            captainController.GotoStep(CaptainController.STEP.CODE);
        }
        else
        {
            captainController.DisplayWrongPassword();
        }
    }

    private void OnProposeCode(string code)
    {
        if ((GameVersion.IsVersionA && code == "111221") || (!GameVersion.IsVersionA && code == "111221"))
        {
            leftPartPnl.SetActive(false);
            captainController.GotoStep(CaptainController.STEP.PRINCIPAL_MISSION);
        }
        else
        {
            captainController.DisplayWrongCode();
        }
    }

    private void OnProposeSendReport()
    {
        captainController.GotoStep(CaptainController.STEP.FINAL_CHOOSE);
    }

    private void OnProposeFinalChoose()
    {
        captainController.GotoStep(CaptainController.STEP.CONGRATULATION);
    }
}
