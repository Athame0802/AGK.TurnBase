using System;
using System.Collections.Generic;
using System.Text;


public class HintCommand : ICommand
{
    public ulong TargetClientId { get; } 

    public HintCommand(ulong targetId)
    {
        TargetClientId = targetId;
    }

    public void Execute()
    {
        CommandManager.Instance.EnqueueCommandQueueRpc(TargetClientId, CommandType.hint);
        CommandManager.Instance.SendHintToClientRpc(TargetClientId);
    }

    public void Undo()
    {
        // 없음
    }
}
