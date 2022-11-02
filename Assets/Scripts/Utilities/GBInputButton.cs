using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class GBInputButton : MonoBehaviour
{
    public bool useResultText = false;
    public bool interactable
    {
        get
        {
            return button.interactable;
        }
        set
        {
            button.interactable = value;
            input.interactable = value;
        }
    }
    public string text
    {
        get
        {
            return input.text;
        }

        set
        {
            input.SetTextWithoutNotify(value);
        }
    }

    [HideInInspector] public UnityEvent onClick;
    [HideInInspector] public TMP_InputField.OnChangeEvent onValueChanged;

    [SerializeField] private Image inputSquareImg, buttonImg;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI placeholderTxt, resultTxt;
    [SerializeField] private TMP_InputField input;

    private bool isCaptain = false;

    private void OnEnable()
    {
        button.onClick.AddListener(OnBtnClick);
        input.onValueChanged.AddListener(OnInputChange);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnBtnClick);
        input.onValueChanged.AddListener(OnInputChange);
    }

    private void Start()
    {
        resultTxt.gameObject.SetActive(useResultText);
    }

    public void SetCaptainMode(bool isCapt)
    {
        placeholderTxt.text = isCapt ? "Enter the code..." : "Wait for the captain to enter the code...";
        button.gameObject.SetActive(isCapt);
        input.interactable = isCapt;

        isCaptain = isCapt;
    }

    public void SetHasNeutral()
    {
        inputSquareImg.color = Color.black;
        buttonImg.color = Color.black;

        input.gameObject.SetActive(false);
    }

    public void SetHasGood(string info)
    {
        inputSquareImg.color = ColorPalette.goodAnswerColor;
        buttonImg.color = ColorPalette.goodAnswerColor;
        resultTxt.color = ColorPalette.goodAnswerColor;

        input.gameObject.SetActive(true);

        resultTxt.text = info;
    }

    public void SetHasBad(string info)
    {
        inputSquareImg.color = ColorPalette.badAnswerColor;
        buttonImg.color = ColorPalette.badAnswerColor;
        resultTxt.color = ColorPalette.badAnswerColor;

        input.gameObject.SetActive(true);

        resultTxt.text = info;
    }

    private void OnBtnClick()
    {
        if (text != string.Empty)
        {
            onClick?.Invoke();
        }
    }

    private void OnInputChange(string value)
    {
        if (isCaptain)
        {
            onValueChanged?.Invoke(value);
        }
    }
}
