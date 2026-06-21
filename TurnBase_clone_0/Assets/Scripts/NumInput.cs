using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NumInput : MonoBehaviour
{
    public static NumInput Instance;

    [SerializeField] private ErrorText errorTextComp;
    [SerializeField] private TMP_InputField inputField;
    private byte[] numbers;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        inputField.onSubmit.AddListener(OnSubmit);
    }

    private void Start()
    {
        TurnManager.Instance.turnClientId.OnValueChanged += OnTurnEnd;

        bool isTurn = TurnManager.Instance.turnClientId.Value == NetworkManager.Singleton.LocalClientId;
        inputField.interactable = isTurn;
    }

    private void OnDestroy()
    {
        TurnManager.Instance.turnClientId.OnValueChanged -= OnTurnEnd;
    }

    private void OnSubmit(string inputStr)
    {
        D.Log($"숫자 입력됨 : {inputStr}");
        string[] inputCut = new string[CorrectNumber.digit]; 

        bool isSucceedInCut = TryCutInput(inputStr, out inputCut);
        if (!isSucceedInCut) return;

        bool isCorrectInput = TryParseChars(inputCut, out numbers);
        if (!isCorrectInput) return;

        CheckAnswerManager.Instance.CheckAnswerRpc(numbers);

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

    private void OnTurnEnd(ulong pastId, ulong currentId)
    {
        bool isTurn = currentId == NetworkManager.Singleton.LocalClientId;
        inputField.interactable = isTurn;
    }

    public void RefreshCharacterLimit()
    {
        inputField.characterLimit = CorrectNumber.digit;
        D.Log($"[Input] 글자 수 제한이 {CorrectNumber.digit}으로 설정됨!");
    }
}
