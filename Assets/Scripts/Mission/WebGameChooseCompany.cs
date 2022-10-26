
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using System;

public class WebGameChooseCompany : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private CaptainController captainController;

    private void OnEnable()
    {
        captainController.onProposePassword += OnProposePassword;
        captainController.onProposeCode += OnProposeCode;
        captainController.onProposeSendReport += OnProposeSendReport;
        captainController.onProposeFinalChoose += OnProposeFinalChoose;
    }
    
    private void OnDisable()
    {
        captainController.onProposePassword -= OnProposePassword;
        captainController.onProposeCode -= OnProposeCode;
        captainController.onProposeSendReport -= OnProposeSendReport;
        captainController.onProposeFinalChoose -= OnProposeFinalChoose;
    }

    private void OnProposePassword(string obj)
    {
        throw new NotImplementedException();
    }

    private void OnProposeCode(string obj)
    {
        throw new NotImplementedException();
    }

    private void OnProposeSendReport()
    {
        throw new NotImplementedException();
    }

    private void OnProposeFinalChoose()
    {
        throw new NotImplementedException();
    }
}
