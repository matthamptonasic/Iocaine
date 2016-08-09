using System;

namespace Iocaine2.Data.Structures
{
    public interface IExecutableAction
    {
        bool IsBlocking { get; }
        bool IsCapable();
        bool CanPerform();
        bool Execute(string iTarget = "");
    }
}
