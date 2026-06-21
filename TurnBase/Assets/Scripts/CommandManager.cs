using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CommandManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private List<Vector3> hideLocations;
    [SerializeField] private GameObject hidePanel;

    public static CommandManager Instance;
    public Queue<ICommand> CommandQueue = new(5);

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

        hidePanel.SetActive(false);
    }

    private void Start()
    {
        CheckAnswerManager.Instance.OnTurnEnd += DequeueCommandQueueRpc;
    }

    private void OnNetworkDestroy()
    {
        CheckAnswerManager.Instance.OnTurnEnd -= DequeueCommandQueueRpc;
    }

    [Rpc(SendTo.Server)]
    public void EnqueueCommandQueueRpc(ulong targetId, CommandType commandType)
    {
        ICommand command;
        switch (commandType)
        {
            case CommandType.hint:
                command = new HintCommand(targetId);
                break;
            case CommandType.hideNumber:
                command = new HideNumberCommand(targetId);
                break;
            default:
                command = null;
                break;
        }

        CommandQueue.Enqueue(command);
    }

    [Rpc(SendTo.Server)]
    public void SendHintToClientRpc(ulong id)
    {
        int numIndex = UnityEngine.Random.Range(0, CorrectNumber.digit - 1);

        SendHintRpc(numIndex, CorrectNumber.correctNumbers[numIndex], RpcTarget.Single(id, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendHintRpc(int numIndex, int number, RpcParams rpcParams = default)
    {
        hintText.text = $"힌트 : {numIndex}번째 자리는 {number}";
    }

    [Rpc(SendTo.Server)]
    public void SendHideNumberToClientRpc(ulong id)
    { 
        int numIndex = UnityEngine.Random.Range(0, 2);

        SendHideNumberRpc(numIndex, RpcTarget.Single(id, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendHideNumberRpc(int numIndex, RpcParams rpcParams = default)
    {
        hidePanel.SetActive(true);
        hidePanel.transform.position = hideLocations[numIndex];
    }

    [Rpc(SendTo.Server)]
    public void SendUndoHideNumberRpc(ulong id)
    {
        SendUndoHideNumberRpc(RpcTarget.Single(id, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SendUndoHideNumberRpc(RpcParams rpcParams = default)
    {
        hidePanel.SetActive(false);
    }

    [Rpc(SendTo.Server)]
    private void DequeueCommandQueueRpc()
    {
        if (CommandQueue.Count <= 0) return;

        ICommand command = CommandQueue.Peek();
        
        if (command.TargetClientId != TurnManager.Instance.turnClientId.Value)
        {
            command.Undo();
            CommandQueue.Dequeue();
        }
    }

    //public void CommandEnqueue(ICommand command)
    //{
    //    commandQueue.Value.Enqueue(command);
    //    EnqueueEventInvokeRpc(command);
    //}

    //[Rpc(SendTo.NotServer)]
    //public void EnqueueEventInvokeRpc(ICommand command)
    //{
    //    OnCommandEnqueue?.Invoke(command);
    //}

    //public ICommand CommandDequeue()
    //{
    //    if (commandQueue.Value.Count <= 0) return null;

    //    ICommand command = commandQueue.Value.Dequeue();
    //    DequeueEventInvokeRpc(command);
    //    return command;

    //}

    //[Rpc(SendTo.NotServer)]
    //public void DequeueEventInvokeRpc(ICommand command)
    //{
    //    OnCommandDequeue?.Invoke(command);
    //}
}
