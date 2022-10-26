using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WebGameVoteCaptain : MissionManager
{
    private const string WGVC_SHAREVOTE = "WGVC_ShareVote";
    private const string WGVC_PROPOSE = "WGVC_Propose";

    [Header("VoteCaptain")]
    [SerializeField] private GameObject waitForPlayer;
    [SerializeField] private TMP_Dropdown playerDpd;
    [SerializeField] private Button validateBtn;

    protected override void OnEnable()
    {
        base.OnEnable();

        Main.UserConnectedManager.OnPlayersChanged += OnPlayersChanged;

        validateBtn.onClick.AddListener(OnValidateClick);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Main.UserConnectedManager.OnPlayersChanged -= OnPlayersChanged;

        validateBtn.onClick.RemoveListener(OnValidateClick);
    }

    protected override void Start()
    {
        base.Start();

        Main.SocketIOManager.Instance.On(WGVC_SHAREVOTE, (string data) =>
        {
            WGVC_Data wgvcData = JsonUtility.FromJson<WGVC_Data>(data);
            FillVote(wgvcData);

            modalsController.CloseModal();
            validationController.DisplayVoteInProgressPanel(ValidationController.VOTE_STEP.DO_YOU_AGREE);
        });

        validateBtn.interactable = false;
        waitForPlayer.SetActive(true);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        Main.SocketIOManager.Instance.Off(WGVC_SHAREVOTE);
    }

    private void OnPlayersChanged(Player[] players)
    {
        playerDpd.ClearOptions();

        List<TMP_Dropdown.OptionData> playerOptions = new List<TMP_Dropdown.OptionData>();
        foreach (Player p in players)
        {
            playerOptions.Add(new TMP_Dropdown.OptionData(p.pseudo));
        }
        playerDpd.AddOptions(playerOptions);

        validateBtn.interactable = players.Length >= 2;
        waitForPlayer.SetActive(players.Length < 2);
    }

    private void OnValidateClick()
    {
        Player playerSelected = Array.Find(Main.UserConnectedManager.ConnectedPlayers, p => p.pseudo == playerDpd.captionText.text);
        WGVC_Data wgvcData = new WGVC_Data()
        {
            captain = playerSelected
        };

        modalsController.CloseModal();
        validationController.DisplayVoteInProgressPanel(ValidationController.VOTE_STEP.WAITING);

        FillVote(wgvcData);

        Main.SocketIOManager.Instance.Emit(WGVC_PROPOSE, JsonUtility.ToJson(wgvcData), false);
    }

    private void FillVote(WGVC_Data wgvc)
    {
        validationController.SetTitleProposition("Vote for your captain", wgvc.captain.pseudo);
    }
}
