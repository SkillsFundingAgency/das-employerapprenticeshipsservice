using System;

namespace SFA.DAS.EAS.DasRecruitService.Models
{
    public interface IVacancy
    {
        string Title { get; set; }
        long Reference { get; set; }
        VacancyStatus Status { get; set; }
        DateTime ClosingDate { get; set; }
        string TrainingTitle { get; set; }
        int NumberOfApplications { get; set; }
        string ManageVacancyUrl { get; set; }
    }

    public enum VacancyStatus
        {
            None,
            Draft,
            Live,
            Rejected,
            PendingReview,
            Closed
        }
}