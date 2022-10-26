using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    [Header("ModalController")]
    [SerializeField] protected ModalsController modalsController;
    [SerializeField] protected ValidationController validationController;

    protected virtual void Awake()
    {
    }

    protected virtual void OnEnable()
    {
        validationController.OnYesAgreement += OnYesAgreeClicked;
        validationController.OnNoAgreement += OnNoAgreeClicked;
    }

    protected virtual void OnDisable()
    {
        validationController.OnYesAgreement -= OnYesAgreeClicked;
        validationController.OnNoAgreement -= OnNoAgreeClicked;
    }

    protected virtual void Start()
    {
        Main.SocketIOManager.Instance.On("disconnect", (string payload) =>
        {
            if (payload.Equals("io server disconnect"))
            {
                Debug.LogWarning("Disconnected from server.");
            }
            else
            {
                Debug.LogWarning("We have been unexpecteldy disconnected. This will cause an automatic reconnect. Reason: " + payload);
            }

            SceneManager.LoadScene(SceneName.WEBGAME_JOINROOM);
        });

        Main.SocketIOManager.Instance.On("WG_NextScene", (string data) =>
        {
            WG_NextSceneData nextSceneData = JsonUtility.FromJson<WG_NextSceneData>(data);
            SceneManager.LoadScene(nextSceneData.nextScene);
        });

        Main.SocketIOManager.Instance.On("MX_VoteProgress", (string data) =>
        {
            MX_VoteProgressData voteProgressData = JsonUtility.FromJson<MX_VoteProgressData>(data);
            validationController.UpdateUserVotes(voteProgressData);
        });
        Main.SocketIOManager.Instance.On("MX_VoteFail", (string data) =>
        {
            MX_VoteFailData voteFailData = JsonUtility.FromJson<MX_VoteFailData>(data);
            validationController.DisplayVoteFail();
        });
        Main.SocketIOManager.Instance.On("MX_VoteTimerUpdate", (string data) =>
        {
            TimerData timerData = JsonUtility.FromJson<TimerData>(data);
            validationController.UpdateVoteTimer(timerData.seconds);
        });

        Main.SocketIOManager.Instance.Emit("requestConnectedPlayers");
        Main.SocketIOManager.Instance.Emit("requestScore");
    }

    protected virtual void OnDestroy()
    {
        Main.SocketIOManager.Instance.Off("disconnect");

        Main.SocketIOManager.Instance.Off("WG_NextScene");
        Main.SocketIOManager.Instance.Off("MX_VoteProgress");
        Main.SocketIOManager.Instance.Off("MX_VoteFail");
        Main.SocketIOManager.Instance.Off("MX_VoteTimerUpdate");
    }

    protected void HideModals()
    {
        modalsController.CloseModal();
    }

    private void OnYesAgreeClicked()
    {
        validationController.DisplayVoteInProgressPanel(ValidationController.VOTE_STEP.WAITING);

        AgreementData agree = new AgreementData() { agree = true };
        Main.SocketIOManager.Instance.Emit("MX_UserVote", JsonUtility.ToJson(agree), false);
    }

    private void OnNoAgreeClicked()
    {
        validationController.DisplayVoteInProgressPanel(ValidationController.VOTE_STEP.WAITING);

        AgreementData disagree = new AgreementData() { agree = false };
        Main.SocketIOManager.Instance.Emit("MX_UserVote", JsonUtility.ToJson(disagree), false);
    }
}
