using System;
using System.Linq;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ManagedCompany;
using SFA.DAS.EAS.Infrastructure.Caching;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class ManagedCompanyLookupService : AzureServiceBase<ManagedCompanyLookup>, IManagedCompanyLookupService
    {
        private readonly ICacheProvider _cacheProvider;

        public ManagedCompanyLookupService(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public override string ConfigurationName => "SFA.DAS.EmployerApprenticeshipsService.CompanyLookup";
        public virtual ManagedCompanyLookup GetCompanies()
        {
            var companies = _cacheProvider.Get<ManagedCompanyLookup>(nameof(ManagedCompanyLookup));

            if (companies == null)
            {
                companies = GetDataFromStorage();
                if (companies?.Data != null && companies.Data.Any())
                {
                    _cacheProvider.Set(nameof(ManagedCompanyLookup), companies, new TimeSpan(0, 30, 0));
                }
            }

            return companies;
        }
    }
}