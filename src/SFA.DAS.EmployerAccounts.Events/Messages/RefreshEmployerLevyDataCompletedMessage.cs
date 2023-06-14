using System;

namespace SFA.DAS.EmployerAccounts.Events.Messages;

[Serializable]
[MessageGroup("levydataimport_completed")]
[Obsolete("Please use RefreshEmployerLevyDataCompletedEvent from EmployerFinance")]
public class RefreshEmployerLevyDataCompletedMessage : AccountMessageBase
{ 
    /// <summary>
    /// true if we have imported some levy; otherwise false;
    /// </summary>
    public bool LevyImported { get; set; }

    public short PeriodMonth { get; set; }

    public string PeriodYear { get; set; }

    public RefreshEmployerLevyDataCompletedMessage()
        : base(0, string.Empty, string.Empty)
    {
    }

    public RefreshEmployerLevyDataCompletedMessage(
        long accountId, bool levyImported, short month, string year, DateTime timestamp, string creatorName, string creatorUserRef)
        : base(accountId, creatorName, creatorUserRef)
    {
        PeriodMonth = month;
        PeriodYear = year;
        LevyImported = levyImported;
        CreatedAt = timestamp;
    }
}