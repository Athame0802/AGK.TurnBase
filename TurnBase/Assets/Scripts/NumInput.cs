using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumInput : MonoBehaviour
{
    [SerializeField] private ErrorText errorTextComp;
    [SerializeField] private TMP_InputField inputField;
    private byte[] numbers;

    private void Awake()
    {
        inputField.onSubmit.AddListener(OnSubmit);
        inputField.characterLimit = CorrectNumber.digit;
    }

    private void OnSubmit(string inputStr)
    {
        D.Log($"숫자 입력됨 : {inputStr}");
        string[] inputCut = new string[CorrectNumber.digit]; 

        bool isSucceedInCut = TryCutInput(inputStr, out inputCut);
        if (!isSucceedInCut) return;

        bool isCorrectInput = TryParseChars(inputCut, out numbers);
        if (!isCorrectInput) return;

        CheckAnswerManager.Instance.CheckAnswer(numbers);

        inputField.text = "";
    }

    private bool TryParseChars(string[] inputChars, out byte[] result)
    {
        try
        {
            if (inputChars.Length < CorrectNumber.digit) { result = null; return false; }
            
            int[] tempResultArray = new int[CorrectNumber.digit];
            for (int i = 0; i < CorrectNumber.digit; i++)
            {
                bool isSucceededParsing = int.TryParse(inputChars[i], out tempResultArray[i]);

                if (!isSucceededParsing) { result = null; return false; }
            }

            byte[] tempByteResultArray = new byte[CorrectNumber.digit];
            for (int i = 0; i < CorrectNumber.digit; i++) tempByteResultArray[i] = (byte)tempResultArray[i];
            result = tempByteResultArray;
            return true;
        }
        catch (Exception e)
        {
            D.LogWarning($"플레이어의 입력이 완전하지 않음! : {e.Message}");
            errorTextComp.ShowErrorText("제대로 입력해주세요.");

            result = null;
            return false;
        }
    }

    private bool TryCutInput(string input, out string[] result)
    {
        try
        {
            string[] inputCut = new string[CorrectNumber.digit];
            
            for (int i = 0; i < CorrectNumber.digit; i++)
            {
                inputCut[i] = input.Substring(i, 1);
            }

            result = inputCut;
            return true;
        }
        catch (Exception e)
        {
            D.LogWarning($"플레이어의 입력이 완전하지 않음! : {e.Message}");
            errorTextComp.ShowErrorText("제대로 입력해주세요.");

            result = null;
            return false;
        }
    }
}
