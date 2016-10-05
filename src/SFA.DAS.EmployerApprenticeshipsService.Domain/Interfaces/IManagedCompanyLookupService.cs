using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.FeatureToggle;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.ManagedCompany;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IManagedCompanyLookupService
    {
        ManagedCompanyLookup GetCompanies();
    }
}