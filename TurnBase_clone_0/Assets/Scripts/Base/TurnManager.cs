using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

// 턴 관리
// 턴에 따라 인풋 필드 활성화 비활성화
public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance { get; private set; }
    public NetworkVariable<ulong> turnClientId;
    private List<ulong> clientIdList = new(2);
    private int index = 0;
    public event Action OnTurnReallyChanged;

    public int turnNum { get; private set; } = 0;

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

    private void Start()
    {
        if (!IsServer) return;

        GetAllClientIds(clientIdList);
        CheckAnswerManager.Instance.OnTurnEnd += OnTurnEnd;
        ChangeTurn().Forget();
    }

    private void OnTurnEnd()
    {
        ChangeTurn().Forget();
    }

    public void GetAllClientIds(List<ulong> clientIdList)
    {
        if (!IsServer) return;

        clientIdList.Clear();

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            clientIdList.Add(clientId);
        }
    }

    private async UniTaskVoid ChangeTurn()
    {
        turnClientId.Value = clientIdList[index++];

        if (index >= clientIdList.Count)
        {
            index = 0;
        }

        turnNum++;

        await UniTask.DelayFrame(60);

        OnTurnReallyChanged?.Invoke();
    }
}
