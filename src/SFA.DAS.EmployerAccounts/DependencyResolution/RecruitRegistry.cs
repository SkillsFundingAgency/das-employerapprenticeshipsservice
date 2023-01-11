using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.DependencyResolution;

public class RecruitRegistry : Registry
{
    public RecruitRegistry()
    {
        For<IRecruitService>().Use<RecruitService>();
        For<IRecruitService>().DecorateAllWith<RecruitServiceWithTimeout>();
    }
}