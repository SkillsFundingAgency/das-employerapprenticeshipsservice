using System;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    public class Vacancy
    {
        public string Title { get; set; }
        public long? Reference { get; set; }
        public VacancyStatus Status { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string TrainingTitle { get; set; }
        public int NumberOfApplications { get; set; }
        public string ManageVacancyUrl { get; set; }
    }
}
