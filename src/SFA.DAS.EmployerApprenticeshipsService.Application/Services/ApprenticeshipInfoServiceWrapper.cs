using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Apprenticeships.Api.Types.Exceptions;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Caches;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.NServiceBus;
using Framework = SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse.Framework;
using Standard = SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse.Standard;

namespace SFA.DAS.EAS.Application.Services
{
    public class ApprenticeshipInfoServiceWrapper : IApprenticeshipInfoServiceWrapper
    {
        private const string StandardsKey = "Standards";
        private const string FrameworksKey = "Frameworks";

        private readonly IInProcessCache _cache;
        private readonly string _apprenticeshipInfoServiceApiBase;

        public ApprenticeshipInfoServiceWrapper(IInProcessCache cache, EmployerApprenticeshipsServiceConfiguration config)
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

        public ProvidersView GetProvider(long ukPrn)
        {
            try
            {
                var api = new Providers.Api.Client.ProviderApiClient(_apprenticeshipInfoServiceApiBase);
                var provider = api.Get(ukPrn);
                return MapFrom(provider);
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (EntityNotFoundException)
            {
                return null;
            }
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
                Provider = new Domain.Models.ApprenticeshipProvider.Provider()
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