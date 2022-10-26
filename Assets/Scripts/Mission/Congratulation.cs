using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Congratulation : MonoBehaviour
{
    [Header("Scoring")]
    [SerializeField] private TextMeshProUGUI nbCardFoundTxt;
    [SerializeField] private TextMeshProUGUI nbErrorTxt, scoreTxt, fastestCardTxt;

    [Header("Rating")]
    [SerializeField] private Toggle[] rateStars;
    [SerializeField] private TMP_InputField comment;
    [SerializeField] private GameObject panelRate, panelRateThanks;
    [SerializeField] private GameObject validateRateBtn;

    private int nbStarSelected = -1;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void Start()
    {
        Main.SocketIOManager.Instance.On("Statistics", (string data) =>
        {
        });

        panelRate.SetActive(true);
        panelRateThanks.SetActive(false);
        validateRateBtn.SetActive(false);
    }

    private void OnDestroy()
    {
        Main.SocketIOManager.Instance.Off("requestStatistics");
    }

    public void OnToggleChange(int idStar)
    {
        for (int i = 0; i < rateStars.Length; i++)
        {
            rateStars[i].SetIsOnWithoutNotify(i <= idStar);
        }
        nbStarSelected = idStar;

        validateRateBtn.SetActive(nbStarSelected != -1);
    }

    public void SendRateAndComment()
    {
        /*RateCommentData rateComment = new RateCommentData()
        {
            rateStar = nbStarSelected + 1,
            comment = comment.text
        };

        panelRate.SetActive(false);
        panelRateThanks.SetActive(true);

        Main.SocketIOManager.Instance.Emit("requestRateAndComment", JsonUtility.ToJson(rateComment), false);*/
    }
}
