using System;
using System.Collections.Generic;
using System.Text;

public interface ICommand
{
    public ulong TargetClientId { get; }

    public void Execute();

    public void Undo();
}

public enum CommandType
{
    hint,
    hideNumber
}
