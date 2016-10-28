using SFA.DAS.EAS.Domain.Models.ManagedCompany;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IManagedCompanyLookupService
    {
        ManagedCompanyLookup GetCompanies();
    }
}