using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public struct Brainteaser
{
    public int id;
    public string question;

    public Brainteaser(int _id, string _question)
    {
        id = _id;
        question = _question;
    }
}

public class CaptainController : MonoBehaviour
{
    public bool IsAppPC { get; set; } = false;

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
    public event Action<Brainteaser, string> onProposeBrainteaser;
    public event Action onProposeSendReport;
    public event Action onProposeFinalChoose;
    public event Action<Brainteaser> onAutoProposeRandomBrainteaser;
    public event Action<WGCC_Data> onCaptainInfoChange;

    public enum STEP { PASSWORD, CODE, PRINCIPAL_MISSION, FINAL_CHOOSE };
    private STEP currentStep = STEP.PASSWORD;

    [Header("General")]
    [SerializeField] private Image youAreCaptainImg;
    [SerializeField] private TextMeshProUGUI headerTimerTxt, alternativeTimerBTTxt, leftTimerTxt, sendReportTxt;
    [SerializeField] private Sprite btnExtendSpt, btnRetractSpt;
    [SerializeField] private Button sendReportBtn, retractBtn;
    [SerializeField] private RectTransform carretRetractRct;
    [SerializeField] private GameObject leftPart, headerPnl, alternativeBTPnl;

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
    [SerializeField] private AudioSource brainteaserStressSrc;
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

    private List<Brainteaser> brainteasers = new List<Brainteaser>();
    private Brainteaser currentBrainteaser;

    private RectTransform panelRct;

    private bool isPanelRetracted = false;

    private bool isCaptain = false;

    private bool isMissionBrainteaser = false;
    private bool isMissionTransversal = false;

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
        brainteaserGbib.onValueChanged.AddListener(OnBrainteaserChange);

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
    }

    public void Initialise()
    {
        brainteasers = new List<Brainteaser>();
        if (GameVersion.IsVersionA)
        {
            brainteasers.Add(new Brainteaser(0, "10 black socks, 8 red socks and 6 white socks are mixed in a drawer. The room is dark. How many socks must be extracted at the MINIMUM in order to be SURE of having two socks of the same color?"));
            brainteasers.Add(new Brainteaser(1, "Max and Astrid are siblings. Max says he has as many brothers as sisters. Astrid says she has twice as many brothers as she has sisters. How many siblings are there in total?"));
            brainteasers.Add(new Brainteaser(2, "What gets broken without being held?"));
            brainteasers.Add(new Brainteaser(3, "A man is looking at a photograph of someone. His friend asks who it is. The man replies, “Brothers and sisters, I have none. But that man’s father is my father’s son.”. Who was in the photograph?"));
            brainteasers.Add(new Brainteaser(4, "Guess the next three letters in the series GTNTL."));
            brainteasers.Add(new Brainteaser(5, "As I was going to St. Ives, I met a man with seven wives. Each wife had seven sacks, Each sack had seven cats, Each cat had seven kits. Kits, cats, sacks and wives. How many were going to St. Ives?"));
            brainteasers.Add(new Brainteaser(6, "What 5-letter word becomes shorter when you add two letters to it?"));
            brainteasers.Add(new Brainteaser(7, "A man wanted to encrypt his password but he needed to do it in a way so that he could remember it. He had to use seven characters consisting of letters and numbers only (no symbols like ! or <). In order to remember it, he wrote down “You force heaven to be empty.”. What is his password?"));
            brainteasers.Add(new Brainteaser(8, "If you take my son’s age and multiply it by one and a half times his age you get 24. How old is my son?"));
            brainteasers.Add(new Brainteaser(9, "How many times does the number 1 appears between 0 and 100 ?"));
        }
        else
        {
            brainteasers.Add(new Brainteaser(0, "If 9999 = 4, 8888 = 8, 1816 = 6, 1212 = 0, then 1919 ="));
            brainteasers.Add(new Brainteaser(1, "Turn Me On My Side And I Am Everything. Cut Me In Half And I Am Nothing. What Am I?"));
            brainteasers.Add(new Brainteaser(2, "It goes in dry, it comes out wet, the longer it is in, the stronger it gets. What is it?"));
            brainteasers.Add(new Brainteaser(3, "Which word in the dictionary is spelled incorrectly?"));
            brainteasers.Add(new Brainteaser(4, "Before Mt. Everest was discovered, what was the highest mountain in the world?"));
            brainteasers.Add(new Brainteaser(5, "I am the beginning of sorrow and the end of sickness. You cannot express happiness without me yet I am in the midst of crosses. I am always in risk yet never in danger. You may find me in the sun, but I am never out of darkness."));
            brainteasers.Add(new Brainteaser(6, "Which day comes three days after the day that comes two days after the day that comes immediately after the day that comes two days after Monday?"));
            brainteasers.Add(new Brainteaser(7, "I left my campsite and hiked south for 3 miles. Then I turned east and hiked for 3 miles. I then turned north and hiked for 3 miles, at which time I came upon a bear inside my tent eating my food! What color was the bear?"));
            brainteasers.Add(new Brainteaser(8, "It is 8:40 pm Without looking at your watch, what is the difference in degrees between the small and the big watch hands?"));
            brainteasers.Add(new Brainteaser(9, "How many times a day do a clock’s hands overlap?"));
        }

        youAreCaptainImg.gameObject.SetActive(false);
        finalChooseBtn.gameObject.SetActive(false);

        alternativeBTPnl.SetActive(!IsAppPC);
        retractBtn.gameObject.SetActive(!IsAppPC);
        transversalBtn.gameObject.SetActive(IsAppPC);

        UpdatePanelDisplay();

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
        currentStep = step;

        UpdatePanelDisplay();
    }
    
    public void SetCaptainMode(bool isCapt)
    {
        youAreCaptainImg.gameObject.SetActive(isCapt);
        passwordGbib.SetCaptainMode(isCapt);
        codeGbib.SetCaptainMode(isCapt);
        brainteaserGbib.SetCaptainMode(isCapt);

        sendReportBtn.gameObject.SetActive(isCapt);
        finalChooseBtn.gameObject.SetActive(isCapt);
        sendReportTxt.gameObject.SetActive(isCapt);

        company1Btn.interactable = isCapt;
        company2Btn.interactable = isCapt;
        noCompanyBtn.interactable = isCapt;

        bngL1C1P1Sld.interactable = bngL1C2P1Sld.interactable = bngL2C1P1Sld.interactable = bngL2C2P1Sld.interactable = bngL3C1P1Sld.interactable = bngL3C2P1Sld.interactable =
        bngL1C1P2Sld.interactable = bngL1C2P2Sld.interactable = bngL2C1P2Sld.interactable = bngL2C2P2Sld.interactable = bngL3C1P2Sld.interactable = bngL3C2P2Sld.interactable =
        bngL1C1P3Sld.interactable = bngL1C2P3Sld.interactable = bngL2C1P3Sld.interactable = bngL2C2P3Sld.interactable = bngL3C1P3Sld.interactable = bngL3C2P3Sld.interactable =
        bngL1C1P4Sld.interactable = bngL1C2P4Sld.interactable = bngL2C1P4Sld.interactable = bngL2C2P4Sld.interactable = bngL3C1P4Sld.interactable = bngL3C2P4Sld.interactable = isCapt;

        isCaptain = isCapt;
    }

    public void SetInfoFromCaptain(WGCC_Data data)
    {
        passwordGbib.text = data.password;
        codeGbib.text = data.code;
        brainteaserGbib.text = data.brainteaser;

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

        if (data.company == 0)
        {
            DeselectAllCompanyButtons();
        }
        else if (data.company == 1)
        {
            SelectCompany1();
        }
        else if (data.company == 2)
        {
            SelectCompany2();
        }
        else
        {
            SelectNoCompany();
        }
    }

    public void DisplayWrongPassword()
    {
        passwordGbib.SetHasWrong(string.Empty);
    }

    public void DisplayWrongCode()
    {
        codeGbib.SetHasWrong(string.Empty);
    }

    public void DisplayGoodBrainteaser()
    {
        brainteaserGbib.SetHasGood("<b>Good answer</b>\nYou have won <b>30s</b>.");

        brainteaserStressSrc.Stop();
        continueBtn.gameObject.SetActive(true);
    }

    public void DisplayWrongBrainteaser()
    {
        brainteaserGbib.SetHasWrong("<b>Wrong answer</b>\nYou have lost <b>30s</b>.");

        brainteaserStressSrc.Stop();
        continueBtn.gameObject.SetActive(true);
    }

    private void OnTimerUpdate(int minutes, int seconds, float pct, string minsecStr)
    {
        headerTimerTxt.text = "Presentation to the general management in : <color=#E9041E>" + minsecStr + "</color>";
        finalTimerTxt.text = "You have <b><color=#E9041E>" + minsecStr + "</color></b> min to make your decision";
        leftTimerTxt.text = minsecStr;
    }

    private void UpdatePanelDisplay()
    {
        // Header
        headerPnl.SetActive((currentStep == STEP.CODE || currentStep == STEP.PRINCIPAL_MISSION || isMissionTransversal) && !isMissionBrainteaser);

        // Left part
        if (IsAppPC)
        {
            leftPart.SetActive(currentStep == STEP.PASSWORD || currentStep == STEP.CODE || currentStep == STEP.FINAL_CHOOSE || isMissionBrainteaser);
        }
        timerPnl.SetActive(currentStep == STEP.PASSWORD || currentStep == STEP.CODE || currentStep == STEP.FINAL_CHOOSE);
        brainTeaserLeftPnl.SetActive(isMissionBrainteaser);

        // Right part
        passwordPnl.SetActive(currentStep == STEP.PASSWORD);
        codePnl.SetActive(currentStep == STEP.CODE);
        principalMissionPnl.SetActive(currentStep == STEP.PRINCIPAL_MISSION);
        brainTeaserRightPnl.SetActive(isMissionBrainteaser);
        transversalPnl.SetActive(isMissionTransversal);
        finalChoosePnl.SetActive(currentStep == STEP.FINAL_CHOOSE);

        // Specific
        if (currentStep == STEP.PASSWORD)
        {
            timerMessageTxt.text = "Presentation to management in :";
        }
        else if (currentStep == STEP.FINAL_CHOOSE)
        {
            timerMessageTxt.text = "Presentation to management\nimminent :";
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

    private void OnBrainteaserChange(string bt)
    {
        wgccData.brainteaser = bt;
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
            carretRetractRct.DORotate(new Vector3(0, 0, 180), RECTRACT_SPEED);
            panelRct.DOAnchorPosX(924f, RECTRACT_SPEED);
            isPanelRetracted = true;
        }
        else
        {
            carretRetractRct.DORotate(new Vector3(0, 0, 0), RECTRACT_SPEED);
            panelRct.DOAnchorPosX(0f, RECTRACT_SPEED);
            isPanelRetracted = false;
        }
    }

    #region BrainTeaser
    private void OnBrainTeaserStart(int index)
    {
        // Sélectionne le brainteaser
        if (!IsAppPC && isCaptain)
        {
            Brainteaser randomBrainteaser = brainteasers[UnityEngine.Random.Range(0, brainteasers.Count)];
            currentBrainteaser = randomBrainteaser;

            onAutoProposeRandomBrainteaser?.Invoke(currentBrainteaser);
        }
        if (IsAppPC)
        {
            Brainteaser randomBrainteaser = brainteasers[UnityEngine.Random.Range(0, brainteasers.Count)];
            currentBrainteaser = randomBrainteaser;

            DisplayBrainteaser();
        }
    }

    public void BrainTeaserStartFromCaptain(int index)
    {
        currentBrainteaser = brainteasers.Find(b => b.id == index);

        DisplayBrainteaser();
    }

    private void OnBrainTeaserUpdate(string timer)
    {
        if (!brainteaserGbib.interactable) timer = "-";

        brainteaserTimerTxt.text = "<b>You have " + timer + " seconds</b> to answer this question.";
        alternativeTimerBTTxt.text = "<b>You have " + timer + " seconds</b> to answer this question.";
    }

    private void OnBrainTeaserEnd()
    {
        HideBrainteaser();
    }

    private void DisplayBrainteaser()
    {
        isMissionBrainteaser = true;
        UpdatePanelDisplay();

        brainteaserQuestionTxt.text = currentBrainteaser.question;
        brainteasers.Remove(currentBrainteaser);

        if (isPanelRetracted)
        {
            OnRetractClick();
        }

        brainteaserStressSrc.Play();

        brainteaserGbib.SetHasNeutral();
        brainteaserGbib.text = string.Empty;
        brainteaserGbib.interactable = IsAppPC || isCaptain;
        continueBtn.gameObject.SetActive(false);
    }
    private void HideBrainteaser()
    {
        isMissionBrainteaser = false;
        UpdatePanelDisplay();
    }
    private void OnBrainteaserGbibValidate()
    {
        brainteaserGbib.interactable = false;
        onProposeBrainteaser?.Invoke(currentBrainteaser, brainteaserGbib.text);
    }
    private void OnBrainteaserContinueClick()
    {
        HideBrainteaser();
    }
    #endregion

    #region CompanySelection
    private void OnCompany1Click()
    {
        SelectCompany1();

        wgccData.company = 1;
        onCaptainInfoChange?.Invoke(wgccData);
    }

    private void OnCompany2Click()
    {
        SelectCompany2();

        wgccData.company = 2;
        onCaptainInfoChange?.Invoke(wgccData);
    }

    private void OnNoCompanyClick()
    {
        SelectNoCompany();

        wgccData.company = 3;
        onCaptainInfoChange?.Invoke(wgccData);
    }

    private void SelectCompany1()
    {
        finalChooseBtn.gameObject.SetActive(true);

        DeselectAllCompanyButtons();

        company1Btn.GetComponentInChildren<TextMeshProUGUI>().color = ColorPalette.badAnswerColor;
        company1Btn.GetComponent<Image>().color = ColorPalette.badAnswerColor;
    }

    private void SelectCompany2()
    {
        finalChooseBtn.gameObject.SetActive(true);

        DeselectAllCompanyButtons();

        company2Btn.GetComponentInChildren<TextMeshProUGUI>().color = ColorPalette.badAnswerColor;
        company2Btn.GetComponent<Image>().color = ColorPalette.badAnswerColor;

    }

    private void SelectNoCompany()
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
        isMissionTransversal = !isMissionTransversal;

        if (!isMissionTransversal)
        {
            transversalBtnTxt.text = "Transversal mission";
        }
        else
        {
            transversalBtnTxt.text = "Back";
        }

        UpdatePanelDisplay();
    }
    private void OnTransversal1Click()
    {
        if ((transversal1Gbib.text.Trim().ToLower() == "yen" && GameVersion.IsVersionA) || (transversal1Gbib.text.Trim().ToLower() == "dollar" && !GameVersion.IsVersionA))
        {
            transversal1Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>40s</b>.");
            Main.TimerManager.AddExtraTime(40);
        }
        else
        {
            transversal1Gbib.SetHasWrong("<b>Wrong answer</b>\nYou have lost <b>20s</b>.");
            Main.TimerManager.SubstractExtraTime(20);
        }
        transversal1Gbib.interactable = false;
    }

    private void OnTransversal2Click()
    {
        if ((transversal2Gbib.text.Trim().ToLower() == "green" && GameVersion.IsVersionA) || (transversal2Gbib.text.Trim().ToLower() == "blue" && !GameVersion.IsVersionA))
        {
            transversal2Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>20s</b>.");
            Main.TimerManager.AddExtraTime(20);
        }
        else
        {
            transversal2Gbib.SetHasWrong("<b>Wrong answer</b>\nYou have lost <b>10s</b>.");
            Main.TimerManager.SubstractExtraTime(10);
        }
        transversal2Gbib.interactable = false;
    }

    private void OnTransversal3Click()
    {
        if ((transversal3Gbib.text.Trim().ToLower() == "9" && GameVersion.IsVersionA) || (transversal3Gbib.text.Trim().ToLower() == "6" && !GameVersion.IsVersionA))
        {
            transversal3Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>45s</b>.");
            Main.TimerManager.AddExtraTime(45);
        }
        else
        {
            transversal3Gbib.SetHasWrong("<b>Wrong answer</b>\nYou have lost <b>22s</b>.");
            Main.TimerManager.SubstractExtraTime(22);
        }
        transversal3Gbib.interactable = false;
    }

    private void OnTransversal4Click()
    {
        if ((transversal4Gbib.text.Trim().ToLower() == "108" && GameVersion.IsVersionA) || (transversal4Gbib.text.Trim().ToLower() == "76" && !GameVersion.IsVersionA))
        {
            transversal4Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>15s</b>.");
            Main.TimerManager.AddExtraTime(15);
        }
        else
        {
            transversal4Gbib.SetHasWrong("<b>Wrong answer</b>\nYou have lost <b>7s</b>.");
            Main.TimerManager.SubstractExtraTime(7);
        }
        transversal4Gbib.interactable = false;
    }

    private void OnTransversal5Click()
    {
        if ((transversal5Gbib.text.Trim().ToLower() == "1527" && GameVersion.IsVersionA) || (transversal5Gbib.text.Trim().ToLower() == "1517" && !GameVersion.IsVersionA))
        {
            transversal5Gbib.SetHasGood("<b>Good answer</b>\nYou have won <b>30s</b>.");
            Main.TimerManager.AddExtraTime(30);
        }
        else
        {
            transversal5Gbib.SetHasWrong("<b>Wrong answer</b>\nYou have lost <b>15s</b>.");
            Main.TimerManager.SubstractExtraTime(15);
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
        extendRectractP1Spt.sprite = content1Pnl.activeSelf ? btnExtendSpt : btnRetractSpt;
        content1Pnl.SetActive(!content1Pnl.activeSelf);
    }

    private void OnExtendRectractP2Click()
    {
        extendRectractP2Spt.sprite = content2Pnl.activeSelf ? btnExtendSpt : btnRetractSpt;
        content2Pnl.SetActive(!content2Pnl.activeSelf);
    }

    private void OnExtendRectractP3Click()
    {
        extendRectractP3Spt.sprite = content3Pnl.activeSelf ? btnExtendSpt : btnRetractSpt;
        content3Pnl.SetActive(!content3Pnl.activeSelf);
    }

    private void OnExtendRectractP4Click()
    {
        extendRectractP4Spt.sprite = content4Pnl.activeSelf ? btnExtendSpt : btnRetractSpt;
        content4Pnl.SetActive(!content4Pnl.activeSelf);
    }
    #endregion
}
