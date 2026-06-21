using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering;

public struct AnswerData : INetworkSerializable, IEquatable<AnswerData>
{
    public ulong inputNumMask;
    public uint AnswerCorrectMask;
    public uint AnswerInaccurateMask;
    public ulong AnsweredClientId;

    public bool Equals(AnswerData other)
    {
        return inputNumMask == other.inputNumMask
            && AnswerCorrectMask == other.AnswerCorrectMask
            && AnswerInaccurateMask == other.AnswerInaccurateMask;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref inputNumMask);
        serializer.SerializeValue(ref AnswerCorrectMask);
        serializer.SerializeValue(ref AnswerInaccurateMask);
    }

    public static ulong PackInput(byte[] inputNum)
    {
        ulong packedInput = 0;

        for (int i = 0; i < CorrectNumber.digit; i++)
        {
            packedInput |= ((ulong)inputNum[i] & 0xF) << (i * 4);
            D.Log($"[CheckAnswer] Input Number Array {i}번째 패킹 중...");
        }

        D.Log($"[CheckAnswer] Input Number Array 패킹 성공!");
        return packedInput;
    }

    public static uint PackBoolArray(bool[] boolArray)
    {
        uint packedBoolArray = 0;

        for (int i = 0; i < CorrectNumber.digit; i++)
        {
            if (boolArray[i])
            {
                packedBoolArray |= (uint)1 << i;
                D.Log($"[CheckAnswer] Bool Array {i}번째 패킹 중...");
            }
        }

        D.Log($"[CheckAnswer] Bool Array 패킹 성공!");
        return packedBoolArray;
    }

    public byte GetInput(int index)
    {
        return (byte)((inputNumMask >> (index * 4)) & 0xF);
    }

    public bool IsCorrect(int index)
    {
        return (AnswerCorrectMask & (1 << index)) != 0;
    }

    public bool IsInaccurate(int index)
    {
        return (AnswerInaccurateMask & (1 << index)) != 0;
    }

    public byte[] ToInputArray()
    {
        byte[] result = new byte[CorrectNumber.digit];

        for (int i = 0; i < CorrectNumber.digit; i++)
        {
            result[i] = (byte)GetInput(i);
        }

        return result;
    }

    public bool[] ToCorrectArray()
    {
        bool[] result = new bool[CorrectNumber.digit];

        for (int i = 0; i < CorrectNumber.digit; i++)
        {
            result[i] = IsCorrect(i);
        }

        return result;
    }
}

public class CheckAnswerManager : NetworkBehaviour
{
    public static CheckAnswerManager Instance;
    private bool[] AnswerCorrects;
    private bool[] AnswerInaccurates;
    public NetworkList<AnswerData> AnswerNetworkList;
    public event Action OnTurnEnd;
    
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

    [Rpc(SendTo.Server)]
    public void CheckAnswerRpc(byte[] inputNum, RpcParams rpcParams = default)
    {
        bool isAllCorrect = true;
        bool[] alreadyCheckedArray = new bool[CorrectNumber.digit];
        bool[] alreadyUsedInputArray = new bool[CorrectNumber.digit];
        AnswerCorrects = new bool[CorrectNumber.digit];
        AnswerInaccurates = new bool[CorrectNumber.digit];

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
        
        AnswerData answerData = new AnswerData();
        
        D.Log("[CheckAnswer] 입력된 숫자 패킹 중...");
        answerData.inputNumMask = AnswerData.PackInput(inputNum);

        D.Log("[CheckAnswer] 맞은 자리 패킹 중...");
        answerData.AnswerCorrectMask = AnswerData.PackBoolArray(AnswerCorrects);

        D.Log("[CheckAnswer] 위치가 틀린 자리 패킹 중...");
        answerData.AnswerInaccurateMask = AnswerData.PackBoolArray(AnswerInaccurates);

        D.Log("[CheckAnswer] 클라이언트 아이디 저장 중...");
        answerData.AnsweredClientId = rpcParams.Receive.SenderClientId;

        AnswerNetworkList.Add(answerData);
        D.Log("[CheckAnswer] 성공적으로 답안 리스트에 추가 완료!");

        if (isAllCorrect)
        {
            D.Log("[CheckAnswer] 모든 숫자를 맞춤!");
            GameManager.Instance.OnAllCorrectAnswerRpc();
            return;
        }

        OnTurnEnd?.Invoke();
    }
}
