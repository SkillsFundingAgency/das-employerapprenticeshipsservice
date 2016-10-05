using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.ManagedCompany;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
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
                    _cacheProvider.Set(nameof(List<EmployerInformation>), companies, new TimeSpan(0, 30, 0));
                }
            }

            return companies;
        }
    }
}