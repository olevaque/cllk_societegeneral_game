
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using DG.Tweening;
using TMPro;
using System;

public class ValidationController : MonoBehaviour
{
    public enum VOTE_STEP { DO_YOU_AGREE, WAITING };

    public event Action OnYesAgreement;
    public event Action OnNoAgreement;

    private const float ANIM_DURATION = .35f;

    [Header("VoteGeneral")]
    [SerializeField] private TextMeshProUGUI titleVoteTxt, propositionTxt;
    [SerializeField] private GameObject voteTimerGo;
    [SerializeField] private TextMeshProUGUI voteTimerTxt;

    [Header("VoteInProgressPanel")]
    [SerializeField] private GameObject voteInProgressPanel;
    [SerializeField] private GameObject agreementDoYouAgreePanel;
    [SerializeField] private GameObject agreementWaitingPanel;

    [Header("VoteInProgressElems")]
    [SerializeField] private TextMeshProUGUI waitingMessage;
    [SerializeField] private Transform userVotesContainer;
    [SerializeField] private GameObject prefabUserVotes;
    [SerializeField] private Sprite waitVoteSpt, voteDoneSpt;
    [SerializeField] private Button noAgreeBtn;
    [SerializeField] private Button yesAgreeBtn;

    [Header("VoteFail")]
    [SerializeField] private GameObject voteFailPanel;
    
    private CanvasGroup validationArea;

    private void Awake()
    {
        validationArea = GetComponent<CanvasGroup>();
        validationArea.alpha = 0;
        validationArea.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        noAgreeBtn.onClick.AddListener(OnNoAgreementClicked);
        yesAgreeBtn.onClick.AddListener(OnYesAgreementClicked);
    }

    private void OnDisable()
    {
        noAgreeBtn.onClick.RemoveListener(OnNoAgreementClicked);
        yesAgreeBtn.onClick.RemoveListener(OnYesAgreementClicked);
    }

    private void Start()
    {
        HideAllAgreementPanels();
    }

    private void OnDestroy()
    {
    }

    public void OpenModal(GameObject modalGo)
    {
        modalGo.SetActive(true);

        validationArea.blocksRaycasts = true;
        validationArea.DOFade(1f, ANIM_DURATION).SetEase(Ease.InOutQuad);
    }

    public void CloseModal()
    {
        validationArea.DOFade(0f, ANIM_DURATION).SetEase(Ease.InOutQuad).OnComplete(() => validationArea.blocksRaycasts = false);
    }

    public void UpdateUserVotes(MX_VoteProgressData voteProgressData)
    {
        // Vide les anciens états des utilisateurs
        foreach(Transform child in userVotesContainer)
        {
            Destroy(child.gameObject);
        }

        // Récreer les états à jour
        int nbHasVoted = 0;
        foreach(UserVote userVote in voteProgressData.userVotes)
        {
            GameObject userVoteGo = Instantiate(prefabUserVotes, userVotesContainer);
            Image voteImg = userVoteGo.GetComponentsInChildren<Image>()[1];
            RectTransform voteImgRect = voteImg.GetComponent<RectTransform>();

            if (userVote.vote != "no-vote")
            {
                nbHasVoted++;
                voteImg.sprite = voteDoneSpt;
            }
            else
            {
                voteImg.sprite = waitVoteSpt;
                voteImgRect.DORotate(new Vector3(0, 0, -360), 1.5f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
            }
            userVoteGo.GetComponentInChildren<TextMeshProUGUI>().text = userVote.player.pseudo;
        }

        waitingMessage.text = "Waiting for all players to cast vote (" + nbHasVoted + "/" + voteProgressData.userVotes.Length + ") :";
    }

    public void UpdateVoteTimer(int seconds)
    {
        voteTimerTxt.text = (seconds < 10 ? "0" + seconds : seconds.ToString()) + "s";
    }

    public void SetTitleProposition(string title, string proposition)
    {
        titleVoteTxt.text = title;

        propositionTxt.gameObject.SetActive(proposition != string.Empty);
        propositionTxt.text = proposition;
    }

    public void DisplayVoteInProgressPanel(VOTE_STEP step)
    {
        HideAllAgreementPanels();

        agreementDoYouAgreePanel.SetActive(step == VOTE_STEP.DO_YOU_AGREE);
        agreementWaitingPanel.SetActive(step == VOTE_STEP.WAITING);

        voteTimerGo.SetActive(step == VOTE_STEP.DO_YOU_AGREE || step == VOTE_STEP.WAITING);

        voteInProgressPanel.SetActive(true);

        validationArea.alpha = 1;
        validationArea.blocksRaycasts = true;
    }

    public void DisplayVoteFail()
    {
        HideAllAgreementPanels();

        voteFailPanel.SetActive(true);
    }

    private void HideAllAgreementPanels()
    {
        voteInProgressPanel.SetActive(false);

        agreementDoYouAgreePanel.SetActive(false);
        agreementWaitingPanel.SetActive(false);

        voteFailPanel.SetActive(false);
    }

    private void OnYesAgreementClicked()
    {
        OnYesAgreement?.Invoke();
    }

    private void OnNoAgreementClicked()
    {
        OnNoAgreement?.Invoke();
    }
}
