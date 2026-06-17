using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckAnswerManager : MonoBehaviour
{
    public static CheckAnswerManager Instance;
    private bool[] AnswerCorrects = new bool[CorrectNumber.digit];
    private bool[] AnswerInaccurates = new bool[CorrectNumber.digit];
    public event Action<byte[], bool[], bool[]> OnAnswerChecked;
    
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
    }

    public void CheckAnswer(byte[] inputNum)
    {
        bool isAllCorrect = true;
        bool[] alreadyCheckedArray = new bool[CorrectNumber.digit];
        bool[] alreadyUsedInputArray = new bool[CorrectNumber.digit];

        for (int i = 0; i < CorrectNumber.digit; i++)
        {
            AnswerCorrects[i] = CorrectNumber.correctNumbers[i] == inputNum[i];

            if (AnswerCorrects[i])
            {
                alreadyUsedInputArray[i] = true;
                alreadyCheckedArray[i] = true;
            }
            else
            {
                D.Log($"정답이 아님 : {i}번 정답 - {CorrectNumber.correctNumbers[i]}, 입력 - {inputNum[i]}");
                isAllCorrect = false;
            }
        }

        for (int i = 0; i < CorrectNumber.digit; i++)
        {
            if (alreadyUsedInputArray[i]) continue;

            for (int j = 0; j < CorrectNumber.digit; j++)
            {
                if (inputNum[i] == CorrectNumber.correctNumbers[j])
                {
                    if (AnswerCorrects[j] || alreadyCheckedArray[j])
                    {
                        continue;
                    }

                    AnswerInaccurates[i] = true;
                    alreadyCheckedArray[j] = true;

                    break;
                }
            }
        }
        
        if (isAllCorrect)
        {
            GameManager.Instance.OnAllCorrectAnswer();
        }

        OnAnswerChecked?.Invoke(inputNum, AnswerCorrects, AnswerInaccurates);
    }
}
