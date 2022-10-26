using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebGameReadMission : MissionManager
{
    private const string WGRM_SHAREVOTE = "WGRM_ShareVote";
    private const string WGRM_PROPOSE = "WGRM_Propose";

    [Header("StartGame")]
    [SerializeField] private Button startBtn;
    [SerializeField] private GameObject waitForPlayer;

    protected override void OnEnable()
    {
        base.OnEnable();

        Main.UserConnectedManager.OnPlayersChanged += OnPlayersChanged;

        startBtn.onClick.AddListener(OnStartClick);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Main.UserConnectedManager.OnPlayersChanged -= OnPlayersChanged;

        startBtn.onClick.RemoveListener(OnStartClick);
    }

    protected override void Start()
    {
        base.Start();

        Main.SocketIOManager.Instance.On(WGRM_SHAREVOTE, (string data) =>
        {
            FillVote();

            modalsController.CloseModal();
            validationController.DisplayVoteInProgressPanel(ValidationController.VOTE_STEP.DO_YOU_AGREE);
        });

        startBtn.interactable = false;
        waitForPlayer.SetActive(true);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Main.SocketIOManager.Instance.Off(WGRM_SHAREVOTE);
    }

    private void OnPlayersChanged(Player[] players)
    {
        startBtn.interactable = players.Length >= 2;
        waitForPlayer.SetActive(players.Length < 2);
    }

    private void OnStartClick()
    {
        FillVote();

        modalsController.CloseModal();
        validationController.DisplayVoteInProgressPanel(ValidationController.VOTE_STEP.WAITING);

        Main.SocketIOManager.Instance.Emit(WGRM_PROPOSE);
    }

    private void FillVote()
    {
        validationController.SetTitleProposition("The team propose to start the game.", string.Empty);
    }
}
