using System.ComponentModel;

namespace SFA.DAS.EAS.Domain.Models.Apprenticeship
{
    public enum ApprenticeshipStatus
    {
        [Description("")]
        None = 0,
        [Description("Waiting to start")]
        WaitingToStart = 1,
        [Description("Live")]
        Live = 2,
        [Description("Paused")]
        Paused = 3,
        [Description("Stopped")]
        Stopped = 4,
        [Description("Finished")]
        Finished = 5
    }
}
