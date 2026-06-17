using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumberGenerator : MonoBehaviour
{
    [SerializeField] private int digit = 4;
    private byte[] numbers;

    private byte minNum = 0;
    private byte maxNum = 9;

    private void Start()
    {
        GenerateNumber(digit, numbers, minNum, maxNum);
    }

    private void GenerateNumber(int digit, byte[] numbers, byte minNum, byte maxNum)
    {
        CorrectNumber.SetDigit(digit);
        numbers = new byte[digit];

        for (int i = 0; i < digit; i++)
        {
            numbers[i] = (byte)Random.Range(minNum, maxNum);
            D.Log($"랜덤 숫자 {i + 1}번째 : {numbers[i]}");
        }

        CorrectNumber.SetCorrectNumbers(numbers);
    }
}
