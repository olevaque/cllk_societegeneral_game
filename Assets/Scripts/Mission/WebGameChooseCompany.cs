
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using System;

public class WebGameChooseCompany : MissionManager
{
    [Header("General")]
    [SerializeField] private CaptainController captainController;
    [SerializeField] private TextMeshProUGUI headerTitleTxt;

    [SerializeField] private Sprite sierraBackgroundA, sierraBackgroundB;
    [SerializeField] private Image sierraBackgroundImg;

    protected override void OnEnable()
    {
        base.OnEnable();

        Main.TimerManager.OnTimerUpdate += OnTimerUpdate;

        captainController.onProposePassword += OnProposePassword;
        captainController.onProposeCode += OnProposeCode;
        captainController.onProposeSendReport += OnProposeSendReport;
        captainController.onProposeFinalChoose += OnProposeFinalChoose;
        captainController.onProposeBrainteaser += OnProposeBrainteaser;
        captainController.onCaptainInfoChange += OnCaptainInfoChange;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Main.TimerManager.OnTimerUpdate -= OnTimerUpdate;

        captainController.onProposePassword -= OnProposePassword;
        captainController.onProposeCode -= OnProposeCode;
        captainController.onProposeSendReport -= OnProposeSendReport;
        captainController.onProposeFinalChoose -= OnProposeFinalChoose;
        captainController.onProposeBrainteaser -= OnProposeBrainteaser;
        captainController.onCaptainInfoChange -= OnCaptainInfoChange;
    }

    private void OnTimerUpdate(int arg1, int arg2, float arg3, string minSecStr)
    {
        headerTitleTxt.text = "Société Générale - " + GameVersion.GetPassword() + " - <color=#E9041E>" + minSecStr + "</color>";
    }

    protected override void Start()
    {
        base.Start();
        
        captainController.Initialise();
        sierraBackgroundImg.sprite = GameVersion.IsVersionA ? sierraBackgroundA : sierraBackgroundB;
        headerTitleTxt.text = "Société Générale - " + GameVersion.GetPassword();

        Main.SocketIOManager.Instance.On("WGCC_CaptainShareInfo", (string data) =>
        {
            WGCC_Data ccData = JsonUtility.FromJson<WGCC_Data>(data);
            captainController.SetInfoFromCaptain(ccData);
        });
        Main.SocketIOManager.Instance.On("WG_NextStep", (string data) =>
        {
            validationController.CloseModal();

            WG_NextStepData nextStepData = JsonUtility.FromJson<WG_NextStepData>(data);
            captainController.GotoStep((CaptainController.STEP)nextStepData.nextStep);
        });
        Main.SocketIOManager.Instance.On("WGCC_CaptainShareBadPass", (string data) =>
        {
            captainController.DisplayWrongPassword();
        });
        Main.SocketIOManager.Instance.On("WGCC_CaptainShareBadCode", (string data) =>
        {
            captainController.DisplayWrongCode();
        });
        Main.SocketIOManager.Instance.On("WGCC_CaptainShareSendReport", (string data) =>
        {
            FillVote();

            modalsController.CloseModal();
            validationController.DisplayVoteInProgressPanel(ValidationController.VOTE_STEP.DO_YOU_AGREE);
        });
        Main.SocketIOManager.Instance.On("WGCC_CaptainShareGoodBrainteaser", (string data) =>
        {
            captainController.DisplayGoodBrainteaser();
        }); 
        Main.SocketIOManager.Instance.On("WGCC_CaptainShareBadBrainteaser", (string data) =>
        {
            captainController.DisplayWrongBrainteaser();
        });
        Main.SocketIOManager.Instance.On("currentCaptain", (string data) =>
        {
            CurrentCaptainData ccData = JsonUtility.FromJson<CurrentCaptainData>(data);
            captainController.SetCaptainMode(ccData.youAreCaptain);
        });

        Main.SocketIOManager.Instance.Emit("requestCurrentCaptain");
        Main.SocketIOManager.Instance.Emit("requestCurrentStep");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        Main.SocketIOManager.Instance.Off("WGCC_CaptainShareInfo");
        Main.SocketIOManager.Instance.Off("WG_NextStep");
        Main.SocketIOManager.Instance.Off("WGCC_CaptainShareBadPass");
        Main.SocketIOManager.Instance.Off("WGCC_CaptainShareBadCode");
        Main.SocketIOManager.Instance.Off("WGCC_CaptainShareSendReport");
        Main.SocketIOManager.Instance.Off("WGCC_CaptainShareGoodBrainteaser");
        Main.SocketIOManager.Instance.Off("WGCC_CaptainShareBadBrainteaser");
        Main.SocketIOManager.Instance.Off("currentCaptain");
    }

    private void OnProposePassword(string pass)
    {
        WGCC_PasswordData ccData = new WGCC_PasswordData()
        {
            password = pass
        };
        Main.SocketIOManager.Instance.Emit("WGCC_CaptainProposePass", JsonUtility.ToJson(ccData), false);
    }

    private void OnProposeCode(string code)
    {
        WGCC_CodeData ccData = new WGCC_CodeData()
        {
            code = code
        };
        Main.SocketIOManager.Instance.Emit("WGCC_CaptainProposeCode", JsonUtility.ToJson(ccData), false);
    }

    private void OnProposeSendReport()
    {
        modalsController.CloseModal();
        validationController.DisplayVoteInProgressPanel(ValidationController.VOTE_STEP.WAITING);

        FillVote();

        Main.SocketIOManager.Instance.Emit("WGCC_CaptainProposeSendReport");
    }

    private void OnProposeFinalChoose()
    {
        Main.SocketIOManager.Instance.Emit("WGCC_CaptainProposeFinalChoose");
    }

    private void OnProposeBrainteaser(int question, string bAnswer)
    {
        WGCC_BrainteaserData ccData = new WGCC_BrainteaserData()
        {
            questionId = question,
            answer = bAnswer.Trim().ToLower()
        };
        Main.SocketIOManager.Instance.Emit("WGCC_CaptainProposeBrainteaser", JsonUtility.ToJson(ccData), false);
    }

    private void OnCaptainInfoChange(WGCC_Data captainInfo)
    {
        Main.SocketIOManager.Instance.Emit("WGCC_CaptainChangeInfo", JsonUtility.ToJson(captainInfo), false);
    }

    private void FillVote()
    {
        validationController.SetTitleProposition("<color=#E20031>The captain</color> wishes to send the report", string.Empty);
    }
}
