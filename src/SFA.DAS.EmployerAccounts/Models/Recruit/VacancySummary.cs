using System;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EmployerAccounts.Models.Recruit
{
    public class VacancySummary
    {
        public string Title { get; set; }
        public long? Reference { get; set; }
        public VacancyStatus Status { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
}