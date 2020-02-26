using System;

namespace SFA.DAS.EmployerAccounts.Models.Recruit
{
    public class Vacancy
    {
        public string Title { get; set; }
        public VacancyStatus Status { get; set; }
        public string ManageVacancyUrl { get; set; }
        public int? NoOfNewApplications { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
}
