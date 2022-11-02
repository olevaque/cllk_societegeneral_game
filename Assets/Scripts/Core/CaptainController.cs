using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class CaptainController : MonoBehaviour
{
    public STEP CurrentStep
    {
        get
        {
            return currentStep;
        }
    }

    private const float RECTRACT_SPEED = .3f;

    public event Action<string> onProposePassword;
    public event Action<string> onProposeCode;
    public event Action onProposeSendReport;
    public event Action onProposeFinalChoose;
    public event Action<WGCC_Data> onCaptainInfoChange;

    public enum STEP { PASSWORD, CODE, PRINCIPAL_MISSION, BRAINTEASER, TRANSVERSAL, FINAL_CHOOSE };
    private STEP currentStep = STEP.PASSWORD;
    private STEP previousStep = STEP.PASSWORD;

    [Header("General")]
    [SerializeField] private Image youAreCaptainImg;
    [SerializeField] private TextMeshProUGUI headerTimerTxt, leftTimerTxt;
    [SerializeField] private Sprite btnExtendSpt, btnRetractSpt;
    [SerializeField] private Button sendReportBtn, retractBtn;
    [SerializeField] private GameObject leftPart, headerPnl;

    [Header("Timer")]
    [SerializeField] private GameObject timerPnl;
    [SerializeField] private TextMeshProUGUI timerMessageTxt;

    [Header("Password")]
    [SerializeField] private GameObject passwordPnl;
    [SerializeField] private GBInputButton passwordGbib;

    [Header("Code")]
    [SerializeField] private GameObject codePnl;
    [SerializeField] private GBInputButton codeGbib;

    [Header("PrincipalMission")]
    [SerializeField] private GameObject principalMissionPnl;
    [SerializeField] private GameObject content1Pnl, content2Pnl, content3Pnl, content4Pnl;
    [SerializeField] private Button extendRectractP1Btn, extendRectractP2Btn, extendRectractP3Btn, extendRectractP4Btn;
    [SerializeField] private Image extendRectractP1Spt, extendRectractP2Spt, extendRectractP3Spt, extendRectractP4Spt;
    [SerializeField] private Image iconC1P1Img, iconC2P1Img, iconC1P2Img, iconC2P2Img, iconC1P3Img, iconC2P3Img, iconC1P4Img, iconC2P4Img;
    [SerializeField] private Slider bngL1C1P1Sld, bngL1C2P1Sld, bngL2C1P1Sld, bngL2C2P1Sld, bngL3C1P1Sld, bngL3C2P1Sld;
    [SerializeField] private Slider bngL1C1P2Sld, bngL1C2P2Sld, bngL2C1P2Sld, bngL2C2P2Sld, bngL3C1P2Sld, bngL3C2P2Sld;
    [SerializeField] private Slider bngL1C1P3Sld, bngL1C2P3Sld, bngL2C1P3Sld, bngL2C2P3Sld, bngL3C1P3Sld, bngL3C2P3Sld;
    [SerializeField] private Slider bngL1C1P4Sld, bngL1C2P4Sld, bngL2C1P4Sld, bngL2C2P4Sld, bngL3C1P4Sld, bngL3C2P4Sld;

    [Header("BrainTeaserPnl")]
    [SerializeField] private GameObject brainTeaserLeftPnl;
    [SerializeField] private GameObject brainTeaserRightPnl;
    [SerializeField] private TextMeshProUGUI brainteaserQuestionTxt, brainteaserTimerTxt;
    [SerializeField] private GBInputButton brainteaserGbib;
    [SerializeField] private Button continueBtn;

    [Header("TransversalPnl")]
    [SerializeField] private GameObject transversalPnl;
    [SerializeField] private TextMeshProUGUI transversalBtnTxt;
    [SerializeField] private Button transversalBtn;
    [SerializeField] private GBInputButton transversal1Gbib, transversal2Gbib, transversal3Gbib, transversal4Gbib, transversal5Gbib;

    [Header("FinalChoosePnl")]
    [SerializeField] private GameObject finalChoosePnl;
    [SerializeField] private TextMeshProUGUI finalTimerTxt, chooseC1Txt, chooseC2Txt;
    [SerializeField] private Button company1Btn, company2Btn, noCompanyBtn, finalChooseBtn;

    private RectTransform panelRct;

    private bool isPanelRetracted = false;
    private int brainteaserIndex = 0;

    WGCC_Data wgccData = new WGCC_Data();

    private void Awake()
    {
        panelRct = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Main.TimerManager.OnTimerUpdate += OnTimerUpdate;
        Main.TimerManager.OnBrainTeaserStart += OnBrainTeaserStart;
        Main.TimerManager.OnBrainTeaserUpdate += OnBrainTeaserUpdate;
        Main.TimerManager.OnBrainTeaserEnd += OnBrainTeaserEnd;

        transversalBtn.onClick.AddListener(OnTransversalClick);

        brainteaserGbib.onClick.AddListener(OnBrainteaserGbibValidate);
        continueBtn.onClick.AddListener(OnBrainteaserContinueClick);

        passwordGbib.onClick.AddListener(OnPasswordClick);
        codeGbib.onClick.AddListener(OnCodeClick);
        sendReportBtn.onClick.AddListener(OnSendReportClick);
        finalChooseBtn.onClick.AddListener(OnFinalChooseClick);

        passwordGbib.onValueChanged.AddListener(OnPasswordChange);
        codeGbib.onValueChanged.AddListener(OnCodeChange);

        extendRectractP1Btn.onClick.AddListener(OnExtendRectractP1Click);
        extendRectractP2Btn.onClick.AddListener(OnExtendRectractP2Click);
        extendRectractP3Btn.onClick.AddListener(OnExtendRectractP3Click);
        extendRectractP4Btn.onClick.AddListener(OnExtendRectractP4Click);

        bngL1C1P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C2P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C1P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C2P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C1P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C2P1Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C1P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C2P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C1P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C2P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C1P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C2P2Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C1P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C2P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C1P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C2P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C1P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C2P3Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C1P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL1C2P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C1P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL2C2P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C1P4Sld.onValueChanged.AddListener(OnBngSliderChange);
        bngL3C2P4Sld.onValueChanged.AddListener(OnBngSliderChange);

        transversal1Gbib.onClick.AddListener(OnTransversal1Click);
        transversal2Gbib.onClick.AddListener(OnTransversal2Click);
        transversal3Gbib.onClick.AddListener(OnTransversal3Click);
        transversal4Gbib.onClick.AddListener(OnTransversal4Click);
        transversal5Gbib.onClick.AddListener(OnTransversal5Click);

        company1Btn.onClick.AddListener(OnCompany1Click);
        company2Btn.onClick.AddListener(OnCompany2Click);
        noCompanyBtn.onClick.AddListener(OnNoCompanyClick);

        retractBtn.onClick.AddListener(OnRetractClick);
    }

    private void OnDisable()
    {
        Main.TimerManager.OnTimerUpdate -= OnTimerUpdate;
        Main.TimerManager.OnBrainTeaserStart -= OnBrainTeaserStart;
        Main.TimerManager.OnBrainTeaserUpdate -= OnBrainTeaserUpdate;
        Main.TimerManager.OnBrainTeaserEnd -= OnBrainTeaserEnd;

        transversalBtn.onClick.RemoveListener(OnTransversalClick);
        continueBtn.onClick.RemoveListener(OnBrainteaserContinueClick);

        passwordGbib.onClick.RemoveListener(OnPasswordClick);
        codeGbib.onClick.RemoveListener(OnCodeClick);
        sendReportBtn.onClick.RemoveListener(OnSendReportClick);
        finalChooseBtn.onClick.RemoveListener(OnFinalChooseClick);

        passwordGbib.onValueChanged.RemoveListener(OnPasswordChange);
        codeGbib.onValueChanged.RemoveListener(OnCodeChange);

        extendRectractP1Btn.onClick.RemoveListener(OnExtendRectractP1Click);
        extendRectractP2Btn.onClick.RemoveListener(OnExtendRectractP2Click);
        extendRectractP3Btn.onClick.RemoveListener(OnExtendRectractP3Click);
        extendRectractP4Btn.onClick.RemoveListener(OnExtendRectractP4Click);

        bngL1C1P1Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL1C2P1Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL2C1P1Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL2C2P1Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL3C1P1Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL3C2P1Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL1C1P2Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL1C2P2Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL2C1P2Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL2C2P2Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL3C1P2Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL3C2P2Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL1C1P3Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL1C2P3Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL2C1P3Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL2C2P3Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL3C1P3Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL3C2P3Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL1C1P4Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL1C2P4Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL2C1P4Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL2C2P4Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL3C1P4Sld.onValueChanged.RemoveListener(OnBngSliderChange);
        bngL3C2P4Sld.onValueChanged.RemoveListener(OnBngSliderChange);

        transversal1Gbib.onClick.RemoveListener(OnTransversal1Click);
        transversal2Gbib.onClick.RemoveListener(OnTransversal2Click);
        transversal3Gbib.onClick.RemoveListener(OnTransversal3Click);
        transversal4Gbib.onClick.RemoveListener(OnTransversal4Click);
        transversal5Gbib.onClick.RemoveListener(OnTransversal5Click);

        company1Btn.onClick.RemoveListener(OnCompany1Click);
        company2Btn.onClick.RemoveListener(OnCompany2Click);
        noCompanyBtn.onClick.RemoveListener(OnNoCompanyClick);

        retractBtn.onClick.RemoveListener(OnRetractClick);
    }

    private void Start()
    {
        youAreCaptainImg.gameObject.SetActive(false);
        finalChooseBtn.gameObject.SetActive(false);

        UpdatePanelDisplay();
    }

    public void Initialise()
    {
        Sprite company1 = GameVersion.GetCompany1Icon();
        Sprite company2 = GameVersion.GetCompany2Icon();
        iconC1P1Img.sprite = company1;
        iconC2P1Img.sprite = company2;
        iconC1P2Img.sprite = company1;
        iconC2P2Img.sprite = company2;
        iconC1P3Img.sprite = company1;
        iconC2P3Img.sprite = company2;
        iconC1P4Img.sprite = company1;
        iconC2P4Img.sprite = company2;

        chooseC1Txt.text = GameVersion.GetCompany1Name();
        chooseC2Txt.text = GameVersion.GetCompany2Name();
    }

    public void GotoStep(STEP step)
    {
        previousStep = currentStep;
        currentStep = step;

        UpdatePanelDisplay();
    }
    
    public void SetCaptainMode(bool isCaptain)
    {
        youAreCaptainImg.gameObject.SetActive(isCaptain);
        passwordGbib.SetCaptainMode(isCaptain);
        codeGbib.SetCaptainMode(isCaptain);

        bngL1C1P1Sld.interactable = bngL1C2P1Sld.interactable = bngL2C1P1Sld.interactable = bngL2C2P1Sld.interactable = bngL3C1P1Sld.interactable = bngL3C2P1Sld.interactable =
        bngL1C1P2Sld.interactable = bngL1C2P2Sld.interactable = bngL2C1P2Sld.interactable = bngL2C2P2Sld.interactable = bngL3C1P2Sld.interactable = bngL3C2P2Sld.interactable =
        bngL1C1P3Sld.interactable = bngL1C2P3Sld.interactable = bngL2C1P3Sld.interactable = bngL2C2P3Sld.interactable = bngL3C1P3Sld.interactable = bngL3C2P3Sld.interactable =
        bngL1C1P4Sld.interactable = bngL1C2P4Sld.interactable = bngL2C1P4Sld.interactable = bngL2C2P4Sld.interactable = bngL3C1P4Sld.interactable = bngL3C2P4Sld.interactable = isCaptain;
    }

    public void SetInfoFromCaptain(WGCC_Data data)
    {
        passwordGbib.text = data.password;
        codeGbib.text = data.code;

        bngL1C1P1Sld.SetValueWithoutNotify(data.bngL1C1P1);
        bngL1C2P1Sld.SetValueWithoutNotify(data.bngL1C2P1);
        bngL2C1P1Sld.SetValueWithoutNotify(data.bngL2C1P1);
        bngL2C2P1Sld.SetValueWithoutNotify(data.bngL2C2P1);
        bngL3C1P1Sld.SetValueWithoutNotify(data.bngL3C1P1);
        bngL3C2P1Sld.SetValueWithoutNotify(data.bngL3C2P1);

        bngL1C1P2Sld.SetValueWithoutNotify(data.bngL1C1P2);
        bngL1C2P2Sld.SetValueWithoutNotify(data.bngL1C2P2);
        bngL2C1P2Sld.SetValueWithoutNotify(data.bngL2C1P2);
        bngL2C2P2Sld.SetValueWithoutNotify(data.bngL2C2P2);
        bngL3C1P2Sld.SetValueWithoutNotify(data.bngL3C1P2);
        bngL3C2P2Sld.SetValueWithoutNotify(data.bngL3C2P2);

        bngL1C1P3Sld.SetValueWithoutNotify(data.bngL1C1P3);
        bngL1C2P3Sld.SetValueWithoutNotify(data.bngL1C2P3);
        bngL2C1P3Sld.SetValueWithoutNotify(data.bngL2C1P3);
        bngL2C2P3Sld.SetValueWithoutNotify(data.bngL2C2P3);
        bngL3C1P3Sld.SetValueWithoutNotify(data.bngL3C1P3);
        bngL3C2P3Sld.SetValueWithoutNotify(data.bngL3C2P3);

        bngL1C1P4Sld.SetValueWithoutNotify(data.bngL1C1P4);
        bngL1C2P4Sld.SetValueWithoutNotify(data.bngL1C2P4);
        bngL2C1P4Sld.SetValueWithoutNotify(data.bngL2C1P4);
        bngL2C2P4Sld.SetValueWithoutNotify(data.bngL2C2P4);
        bngL3C1P4Sld.SetValueWithoutNotify(data.bngL3C1P4);
        bngL3C2P4Sld.SetValueWithoutNotify(data.bngL3C2P4);
    }

    public void DisplayWrongPassword()
    {
        passwordGbib.SetHasBad(string.Empty);
    }

    public void DisplayWrongCode()
    {
        codeGbib.SetHasBad(string.Empty);
    }

    public void UsePC_App()
    {
        retractBtn.gameObject.SetActive(false);
        transversalBtn.gameObject.SetActive(true);
    }

    private void OnTimerUpdate(int minutes, int seconds, float pct, string minsecStr)
    {
        headerTimerTxt.text = "Presentation to the general management in : <color=#E9041E>" + minsecStr + "</color>";
        finalTimerTxt.text = "You have <b><color=#E9041E>" + minsecStr + "</color></b> min to make your decision";
        leftTimerTxt.text = minsecStr;
    }

    private void UpdatePanelDisplay()
    {
        //Header
        headerPnl.SetActive(currentStep == STEP.CODE || currentStep == STEP.PRINCIPAL_MISSION || currentStep == STEP.TRANSVERSAL);

        // Left part
        leftPart.SetActive(currentStep == STEP.PASSWORD || currentStep == STEP.CODE || currentStep == STEP.BRAINTEASER || currentStep == STEP.FINAL_CHOOSE);
        timerPnl.SetActive(currentStep == STEP.PASSWORD || currentStep == STEP.CODE || currentStep == STEP.FINAL_CHOOSE);
        brainTeaserLeftPnl.SetActive(currentStep == STEP.BRAINTEASER);

        // Right part
        passwordPnl.SetActive(currentStep == STEP.PASSWORD);
        codePnl.SetActive(currentStep == STEP.CODE);
        principalMissionPnl.SetActive(currentStep == STEP.PRINCIPAL_MISSION);
        brainTeaserRightPnl.SetActive(currentStep == STEP.BRAINTEASER);
        transversalPnl.SetActive(currentStep == STEP.TRANSVERSAL);
        finalChoosePnl.SetActive(currentStep == STEP.FINAL_CHOOSE);

        // Specific
        if (currentStep == STEP.PASSWORD)
        {
            timerMessageTxt.text = "Presentation to management in :";
        }
        else if (currentStep == STEP.FINAL_CHOOSE)
        {
            timerMessageTxt.text = "Presentation to management imminent :";
        }
    }

    private void OnPasswordChange(string pass)
    {
        wgccData.password = pass;
        onCaptainInfoChange?.Invoke(wgccData);
    }

    private void OnCodeChange(string code)
    {
        wgccData.code = code;
        onCaptainInfoChange?.Invoke(wgccData);
    }

    private void OnBngSliderChange(float arg0)
    {
        wgccData.bngL1C1P1 = (int)bngL1C1P1Sld.value;
        wgccData.bngL1C2P1 = (int)bngL1C2P1Sld.value;
        wgccData.bngL2C1P1 = (int)bngL2C1P1Sld.value;
        wgccData.bngL2C2P1 = (int)bngL2C2P1Sld.value;
        wgccData.bngL3C1P1 = (int)bngL3C1P1Sld.value;
        wgccData.bngL3C2P1 = (int)bngL3C2P1Sld.value;

        wgccData.bngL1C1P2 = (int)bngL1C1P2Sld.value;
        wgccData.bngL1C2P2 = (int)bngL1C2P2Sld.value;
        wgccData.bngL2C1P2 = (int)bngL2C1P2Sld.value;
        wgccData.bngL2C2P2 = (int)bngL2C2P2Sld.value;
        wgccData.bngL3C1P2 = (int)bngL3C1P2Sld.value;
        wgccData.bngL3C2P2 = (int)bngL3C2P2Sld.value;

        wgccData.bngL1C1P3 = (int)bngL1C1P3Sld.value;
        wgccData.bngL1C2P3 = (int)bngL1C2P3Sld.value;
        wgccData.bngL2C1P3 = (int)bngL2C1P3Sld.value;
        wgccData.bngL2C2P3 = (int)bngL2C2P3Sld.value;
        wgccData.bngL3C1P3 = (int)bngL3C1P3Sld.value;
        wgccData.bngL3C2P3 = (int)bngL3C2P3Sld.value;

        wgccData.bngL1C1P4 = (int)bngL1C1P4Sld.value;
        wgccData.bngL1C2P4 = (int)bngL1C2P4Sld.value;
        wgccData.bngL2C1P4 = (int)bngL2C1P4Sld.value;
        wgccData.bngL2C2P4 = (int)bngL2C2P4Sld.value;
        wgccData.bngL3C1P4 = (int)bngL3C1P4Sld.value;
        wgccData.bngL3C2P4 = (int)bngL3C2P4Sld.value;
        onCaptainInfoChange?.Invoke(wgccData);
    }
    
    private void OnRetractClick()
    {
        if (!isPanelRetracted)
        {
            panelRct.DOAnchorPosX(panelRct.sizeDelta.x, RECTRACT_SPEED);
            isPanelRetracted = true;
        }
        else
        {
            panelRct.DOAnchorPosX(0, RECTRACT_SPEED);
            isPanelRetracted = false;
        }
    }

    #region BrainTeaser
    private void OnBrainTeaserStart(int index)
    {
        DisplayBrainteaser(index);
    }

    private void OnBrainTeaserUpdate(string timer)
    {
        brainteaserTimerTxt.text = "<b>You have " + timer + " seconds</b> to answer this question.";
    }

    private void OnBrainTeaserEnd()
    {
        HideBrainteaser();
    }
    private void DisplayBrainteaser(int idBrainTeaser)
    {
        GotoStep(STEP.BRAINTEASER);

        brainteaserIndex = idBrainTeaser;

        if (idBrainTeaser == 0)
        {
            brainteaserQuestionTxt.text = "Which day comes three days after the day that comes two days after the day that comes immediately after the day that comes two days after Monday?";
        }
        else if (idBrainTeaser == 1)
        {
            brainteaserQuestionTxt.text = "10 black socks, 8 red socks and 6 white socks are mixed in a drawer. The room is dark. How many socks must be extracted at the MINIMUM in order to be SURE of having two socks of the same color?";
        }
        else if (idBrainTeaser == 2)
        {
            brainteaserQuestionTxt.text = "Max and Astrid are siblings. Max says he has as many brothers as sisters. Astrid says she has twice as many brothers as she has sisters. How many siblings are there in total?";
        }
        else if (idBrainTeaser == 3)
        {
            brainteaserQuestionTxt.text = "I am 4 times my son's age. In 20 years, I'll be twice his age. How old is my son today ?";
        }

        continueBtn.gameObject.SetActive(false);
    }
    private void HideBrainteaser()
    {
        GotoStep(previousStep);
    }
    private void OnBrainteaserGbibValidate()
    {
        continueBtn.gameObject.SetActive(true);

        if (brainteaserIndex == 0 && brainteaserGbib.text == "tuesday")
        {
            brainteaserGbib.SetHasGood()
        }
        else if (brainteaserIndex == 1)
        {
            brainteaserQuestionTxt.text = "10 black socks, 8 red socks and 6 white socks are mixed in a drawer. The room is dark. How many socks must be extracted at the MINIMUM in order to be SURE of having two socks of the same color?";
        }
        else if (brainteaserIndex == 2)
        {
            brainteaserQuestionTxt.text = "Max and Astrid are siblings. Max says he has as many brothers as sisters. Astrid says she has twice as many brothers as she has sisters. How many siblings are there in total?";
        }
        else if (brainteaserIndex == 3)
        {
            brainteaserQuestionTxt.text = "I am 4 times my son's age. In 20 years, I'll be twice his age. How old is my son today ?";
        }
    }
    private void OnBrainteaserContinueClick()
    {
        HideBrainteaser();
    }
    #endregion

    #region CompanySelection
    private void OnCompany1Click()
    {
        finalChooseBtn.gameObject.SetActive(true);

        DeselectAllCompanyButtons();

        company1Btn.GetComponentInChildren<TextMeshProUGUI>().color = ColorPalette.badAnswerColor;
        company1Btn.GetComponent<Image>().color = ColorPalette.badAnswerColor;
    }

    private void OnCompany2Click()
    {
        finalChooseBtn.gameObject.SetActive(true);

        DeselectAllCompanyButtons();

        company2Btn.GetComponentInChildren<TextMeshProUGUI>().color = ColorPalette.badAnswerColor;
        company2Btn.GetComponent<Image>().color = ColorPalette.badAnswerColor;
    }

    private void OnNoCompanyClick()
    {
        finalChooseBtn.gameObject.SetActive(true);

        DeselectAllCompanyButtons();

        noCompanyBtn.GetComponentInChildren<TextMeshProUGUI>().color = ColorPalette.badAnswerColor;
        noCompanyBtn.GetComponent<Image>().color = ColorPalette.badAnswerColor;
    }

    private void DeselectAllCompanyButtons()
    {
        company1Btn.GetComponent<Image>().color = ColorPalette.neutralColor;
        company2Btn.GetComponent<Image>().color = ColorPalette.neutralColor;
        noCompanyBtn.GetComponent<Image>().color = ColorPalette.neutralColor;

        company1Btn.GetComponentInChildren<TextMeshProUGUI>().color = ColorPalette.neutralColor;
        company2Btn.GetComponentInChildren<TextMeshProUGUI>().color = ColorPalette.neutralColor;
        noCompanyBtn.GetComponentInChildren<TextMeshProUGUI>().color = ColorPalette.neutralColor;
    }
    #endregion

    #region TransversalMission
    private void OnTransversalClick()
    {
        if (currentStep == STEP.TRANSVERSAL)
        {
            transversalBtnTxt.text = "Transversal mission";
            GotoStep(previousStep);
        }
        else
        {
            transversalBtnTxt.text = "Back";
            GotoStep(STEP.TRANSVERSAL);
        }
    }
    private void OnTransversal1Click()
    {
        if (transversal1Gbib.text.Trim().ToLower() == "yen")
        {
            transversal1Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>30s</b>.");
            Main.TimerManager.AddExtraTime(30);
        }
        else
        {
            transversal1Gbib.SetHasBad("<b>Wrong answer</b>\nYou have lost <b>30s</b>.");
            Main.TimerManager.SubstractExtraTime(30);
        }
        transversal1Gbib.interactable = false;
    }

    private void OnTransversal2Click()
    {
        if (transversal2Gbib.text.Trim().ToLower() == "vert")
        {
            transversal2Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>30s</b>.");
            Main.TimerManager.AddExtraTime(30);
        }
        else
        {
            transversal2Gbib.SetHasBad("<b>Wrong answer</b>\nYou have lost <b>30s</b>.");
            Main.TimerManager.SubstractExtraTime(30);
        }
        transversal2Gbib.interactable = false;
    }

    private void OnTransversal3Click()
    {
        if (transversal3Gbib.text.Trim().ToLower() == "9")
        {
            transversal3Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>30s</b>.");
            Main.TimerManager.AddExtraTime(30);
        }
        else
        {
            transversal3Gbib.SetHasBad("<b>Wrong answer</b>\nYou have lost <b>30s</b>.");
            Main.TimerManager.SubstractExtraTime(30);
        }
        transversal3Gbib.interactable = false;
    }

    private void OnTransversal4Click()
    {
        if (transversal4Gbib.text.Trim().ToLower() == "108")
        {
            transversal4Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>30s</b>.");
            Main.TimerManager.AddExtraTime(30);
        }
        else
        {
            transversal4Gbib.SetHasBad("<b>Wrong answer</b>\nYou have lost <b>30s</b>.");
            Main.TimerManager.SubstractExtraTime(30);
        }
        transversal4Gbib.interactable = false;
    }

    private void OnTransversal5Click()
    {
        if (transversal5Gbib.text.Trim().ToLower() == "1527")
        {
            transversal5Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>30s</b>.");
            Main.TimerManager.AddExtraTime(30);
        }
        else
        {
            transversal5Gbib.SetHasBad("<b>Wrong answer</b>\nYou have lost <b>30s</b>.");
            Main.TimerManager.SubstractExtraTime(30);
        }
        transversal5Gbib.interactable = false;
    }
    #endregion

    #region Clicks
    private void OnPasswordClick()
    {
        onProposePassword?.Invoke(passwordGbib.text);
    }

    private void OnCodeClick()
    {
        onProposeCode?.Invoke(codeGbib.text);
    }

    private void OnSendReportClick()
    {
        onProposeSendReport?.Invoke();
    }

    private void OnFinalChooseClick()
    {
        onProposeFinalChoose?.Invoke();
    }

    private void OnExtendRectractP1Click()
    {
        content1Pnl.SetActive(!content1Pnl.activeSelf);
    }

    private void OnExtendRectractP2Click()
    {
        content2Pnl.SetActive(!content2Pnl.activeSelf);
    }

    private void OnExtendRectractP3Click()
    {
        content3Pnl.SetActive(!content3Pnl.activeSelf);
    }

    private void OnExtendRectractP4Click()
    {
        content4Pnl.SetActive(!content4Pnl.activeSelf);
    }
    #endregion
}
