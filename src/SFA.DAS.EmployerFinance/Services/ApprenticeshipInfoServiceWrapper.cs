using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Caches;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse;
using SFA.DAS.EmployerFinance.Models.ApprenticeshipProvider;
using Framework = SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse.Framework;
using Standard = SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse.Standard;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ApprenticeshipInfoServiceWrapper : IApprenticeshipInfoServiceWrapper
    {
        private const string StandardsKey = "Standards";
        private const string FrameworksKey = "Frameworks";

        private readonly IInProcessCache _cache;
        private readonly string _apprenticeshipInfoServiceApiBase;

        public ApprenticeshipInfoServiceWrapper(IInProcessCache cache, EmployerFinanceConfiguration config)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _apprenticeshipInfoServiceApiBase = config?.ApprenticeshipInfoService?.BaseUrl;
        }

        public Task<StandardsView> GetStandardsAsync(bool refreshCache = false)
        {
            if (!_cache.Exists(StandardsKey) || refreshCache)
            {
                var api = new StandardApiClient(_apprenticeshipInfoServiceApiBase);

                //BUG: FindAll should be FindAllAsync - currently a blocking call.
                var standards = api.FindAll().OrderBy(x => x.Title).ToList();

                _cache.Set(StandardsKey, MapFrom(standards));
            }

            return Task.FromResult(_cache.Get<StandardsView>(StandardsKey));
        }

        public Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false)
        {
            if (!_cache.Exists(FrameworksKey) || refreshCache)
            {
                var api = new FrameworkApiClient(_apprenticeshipInfoServiceApiBase);

                //BUG: FindAll should be FindAllAsync
                var frameworks = api.FindAll().OrderBy(x => x.Title).ToList();

                _cache.Set(FrameworksKey, MapFrom(frameworks));
            }

            return Task.FromResult(_cache.Get<FrameworksView>(FrameworksKey));
        }       

        private static FrameworksView MapFrom(List<FrameworkSummary> frameworks)
        {
            return new FrameworksView
            {
                CreatedDate = DateTime.UtcNow,
                Frameworks = frameworks.Select(x => new Framework
                {
                    Id = x.Id,
                    Title = GetTitle(x.FrameworkName.Trim() == x.PathwayName.Trim() ? x.FrameworkName : x.Title, x.Level),
                    FrameworkCode = x.FrameworkCode,
                    FrameworkName = x.FrameworkName,
                    ProgrammeType = x.ProgType,
                    Level = x.Level,
                    PathwayCode = x.PathwayCode,
                    PathwayName = x.PathwayName,
                    Duration = x.Duration,
                    MaxFunding = x.MaxFunding
                }).ToList()
            };
        }

        private static ProvidersView MapFrom(Apprenticeships.Api.Types.Providers.Provider provider)
        {
            return new ProvidersView
            {
                CreatedDate = DateTime.UtcNow,
                Provider = new Models.ApprenticeshipProvider.Provider()
                {
                    Ukprn = provider.Ukprn,
                    ProviderName = provider.ProviderName,
                    Email = provider.Email,
                    Phone = provider.Phone,
                    NationalProvider = provider.NationalProvider
                }
            };
        }

        private static StandardsView MapFrom(List<StandardSummary> standards)
        {
            return new StandardsView
            {
                CreationDate = DateTime.UtcNow,
                Standards = standards.Select(x => new Standard
                {
                    Id = x.Id,
                    Code = long.Parse(x.Id),
                    Level = x.Level,
                    Title = GetTitle(x.Title, x.Level) + " (Standard)",
                    CourseName = x.Title,
                    Duration = x.Duration,
                    MaxFunding = x.MaxFunding
                }).ToList()
            };
        }

        private static string GetTitle(string title, int level)
        {
            return $"{title}, Level: {level}";
        }
    }
}