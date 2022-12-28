using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AppGame : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private CaptainController captainController;

    [Header("ChooseGameVersion")]
    [SerializeField] private GameObject chooseVersionPnl;
    [SerializeField] private Button playVersionA, playVersionB;

    private void Awake()
    {
        chooseVersionPnl.SetActive(true);
    }

    private void OnEnable()
    {
        captainController.onProposePassword += OnProposePassword;
        captainController.onProposeCode += OnProposeCode;
        captainController.onProposeSendReport += OnProposeSendReport;
        captainController.onProposeFinalChoose += OnProposeFinalChoose;
        captainController.onProposeBrainteaser += OnProposeBrainTeaser;

        playVersionA.onClick.AddListener(OnVersionAClick);
        playVersionB.onClick.AddListener(OnVersionBClick);

        Main.TimerManager.OnTimerComplete += OnTimerComplete;
    }

    private void OnDisable()
    {
        captainController.onProposePassword -= OnProposePassword;
        captainController.onProposeCode -= OnProposeCode;
        captainController.onProposeSendReport -= OnProposeSendReport;
        captainController.onProposeFinalChoose -= OnProposeFinalChoose;
        captainController.onProposeBrainteaser -= OnProposeBrainTeaser;

        playVersionA.onClick.RemoveListener(OnVersionAClick);
        playVersionB.onClick.RemoveListener(OnVersionBClick);

        Main.TimerManager.OnTimerComplete -= OnTimerComplete;
    }

    private void Start()
    {
        // Spécifique au jeu solo
        Main.SocketIOManager.enabled = false;
        captainController.IsAppPC = true;

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
        Main.TimerManager.StartTimer40min();

        captainController.Initialise();

        chooseVersionPnl.SetActive(false);
    }

    private void OnTimerComplete()
    {
        if (captainController.CurrentStep == CaptainController.STEP.FINAL_CHOOSE)
        {
            SceneManager.LoadScene(SceneName.APPGAME_CONGRATULATION);
        }
        else
        {
            StartFinalChoose();
        }
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
            captainController.GotoStep(CaptainController.STEP.PRINCIPAL_MISSION);
        }
        else
        {
            captainController.DisplayWrongCode();
        }
    }

    private void OnProposeBrainTeaser(Brainteaser brainteaser, string answer)
    {
        answer = answer.Trim().ToLower();

        if (    (GameVersion.IsVersionA && brainteaser.id == 0 && answer == "4") ||
                (GameVersion.IsVersionA && brainteaser.id == 1 && answer == "7") ||
                (GameVersion.IsVersionA && brainteaser.id == 2 && answer.Contains("promise")) ||
                (GameVersion.IsVersionA && brainteaser.id == 3 && answer.Contains("son")) ||
                (GameVersion.IsVersionA && brainteaser.id == 4 && answer == "its") ||
                (GameVersion.IsVersionA && brainteaser.id == 5 && answer == "one") ||
                (GameVersion.IsVersionA && brainteaser.id == 6 && answer == "short") ||
                (GameVersion.IsVersionA && brainteaser.id == 7 && answer == "u472bmt") ||
                (GameVersion.IsVersionA && brainteaser.id == 8 && answer == "4") ||
                (GameVersion.IsVersionA && brainteaser.id == 9 && answer == "21") ||

                (!GameVersion.IsVersionA && brainteaser.id == 0 && answer == "4") ||
                (!GameVersion.IsVersionA && brainteaser.id == 1 && answer == "8") ||
                (!GameVersion.IsVersionA && brainteaser.id == 2 && answer.Contains("tea bag")) ||
                (!GameVersion.IsVersionA && brainteaser.id == 3 && answer == "incorrectly") ||
                (!GameVersion.IsVersionA && brainteaser.id == 4 && answer.Contains("everest")) ||
                (!GameVersion.IsVersionA && brainteaser.id == 5 && answer == "s") ||
                (!GameVersion.IsVersionA && brainteaser.id == 6 && answer == "tuesday") ||
                (!GameVersion.IsVersionA && brainteaser.id == 7 && answer == "white") ||
                (!GameVersion.IsVersionA && brainteaser.id == 8 && answer == "20") ||
                (!GameVersion.IsVersionA && brainteaser.id == 9 && answer == "22")
                )
        {
            captainController.DisplayGoodBrainteaser();
            Main.TimerManager.AddExtraTime(30);
        }
        else
        {
            captainController.DisplayWrongBrainteaser();
            Main.TimerManager.SubstractExtraTime(30);
        }
    }

    private void OnProposeSendReport()
    {
        StartFinalChoose();
    }

    private void OnProposeFinalChoose()
    {
        StartCongratulation();
    }

    private void StartFinalChoose()
    {
        Main.TimerManager.StopTimer();
        Main.TimerManager.StartTimer1m30();

        captainController.GotoStep(CaptainController.STEP.FINAL_CHOOSE);
    }

    private void StartCongratulation()
    {
        Main.TimerManager.StopTimer();
        SceneManager.LoadScene(SceneName.APPGAME_CONGRATULATION);
    }
}
