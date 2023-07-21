namespace SFA.DAS.EmployerAccounts.Models.Recruit;

public enum VacancyStatus
{
    Draft,
    Review,
    Rejected, // Rejected by employer during "Collaborate"
    Submitted,
    Referred, // Rejected by QA
    Live,
    Closed,
    Approved
}