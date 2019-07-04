using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.EAS.DasRecruitService.Models;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    class Vacancy : IVacancy
    {
        public string Title { get; set; }
        public long Reference { get; set; }
        public VacancyStatus Status { get; set; }
        public DateTime ClosingDate { get; set; }
        public string TrainingTitle { get; set; }
        public int NumberOfApplications { get; set; }
        public string ManageVacancyUrl { get; set; }
    }
}
