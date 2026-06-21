using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;


public class HideNumberCommand : ICommand
{
    public ulong TargetClientId { get; }

    public HideNumberCommand(ulong id)
    {
        TargetClientId = id;
    }

    public void Execute()
    {
        CommandManager.Instance.EnqueueCommandQueueRpc(TargetClientId, CommandType.hideNumber);
        CommandManager.Instance.SendHideNumberToClientRpc(TargetClientId);
    }

    public void Undo()
    {
        CommandManager.Instance.SendUndoHideNumberRpc(TargetClientId);
    }
}
