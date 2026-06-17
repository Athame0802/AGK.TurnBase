using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CorrectNumber
{
    public static int digit { get; private set; } = 4;
    public static byte[] correctNumbers { get; private set; }
    
    public static void SetCorrectNumbers(byte[] numbers)
    {
        correctNumbers = new byte[digit];

        for (int i = 0; i < digit; i++)
        {
            correctNumbers[i] = numbers[i]; 
        }
    }

    public static void SetDigit(int digit)
    {
        D.Log($"정답 자릿수가 변경됨 : {CorrectNumber.digit} -> {digit}");
        CorrectNumber.digit = digit;
    }
}