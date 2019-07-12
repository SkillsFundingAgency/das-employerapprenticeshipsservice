using System;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    [Flags]
    public enum AccountState : short
    {
        None = 0,
        HasPayeScheme = 1
    }
}