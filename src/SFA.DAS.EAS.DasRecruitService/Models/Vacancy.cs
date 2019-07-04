using System;

namespace SFA.DAS.EAS.DasRecruitService.Models
{
    public class Vacancy : IVacancy
    {
        public string Title { get; set; }
        public long Reference { get; set; }
        public VacancyStatus Status { get; set; }
        public DateTime ClosingDate { get; set; }
        public string TrainingTitle { get; set; }
        public int NumberOfApplications { get; set; }
    }
}
