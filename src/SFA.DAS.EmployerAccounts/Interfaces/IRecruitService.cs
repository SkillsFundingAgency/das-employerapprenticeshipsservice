using SFA.DAS.EmployerAccounts.Models.Recruit;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IRecruitService
{
    Task<IEnumerable<Vacancy>> GetVacancies(string hashedAccountId, int maxVacanciesToGet = int.MaxValue);
}