using System;

namespace Iocaine2.Data.Structures
{
    public interface IExecutableAction
    {
        bool IsCapable();
        bool CanPerform();
        bool Execute(string iTarget = "");
    }
}
