using SFA.DAS.EAS.Domain.Interfaces;
using System;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public static class ApprenticeshipExtensions
    {
        public static bool IsWaitingToStart(this Apprenticeship apprenticeship, ICurrentDateTime currentDateTime)
        {
            return apprenticeship.StartDate.Value > new DateTime(currentDateTime.Now.Year, currentDateTime.Now.Month, 1);
        }
    }
}