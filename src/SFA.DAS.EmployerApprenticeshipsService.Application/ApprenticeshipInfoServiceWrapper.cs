using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using Framework = SFA.DAS.EAS.Domain.Framework;
using Provider = SFA.DAS.EAS.Domain.Provider;

namespace SFA.DAS.EAS.Application
{
    public class ApprenticeshipInfoServiceWrapper : IApprenticeshipInfoServiceWrapper
    {
        private const string StandardsKey = "Standards";
        private const string FrameworksKey = "Frameworks";

        private readonly ICache _cache;
        private readonly IApprenticeshipInfoServiceConfiguration _configuration;

        public ApprenticeshipInfoServiceWrapper(ICache cache, IApprenticeshipInfoServiceConfiguration configuration)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<StandardsView> GetStandardsAsync(bool refreshCache = false)
        {
            if (!await _cache.ExistsAsync(StandardsKey) || refreshCache)
            {
                var api = new StandardApiClient(_configuration.BaseUrl);

                var standards = api.FindAll().OrderBy(x => x.Title).ToList();

                await _cache.SetCustomValueAsync(StandardsKey, MapFrom(standards));
            }

            return await _cache.GetCustomValueAsync<StandardsView>(StandardsKey);
        }

        public async Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false)
        {
            if (!await _cache.ExistsAsync(FrameworksKey) || refreshCache)
            {
                var api = new FrameworkApiClient(_configuration.BaseUrl);

                var frameworks = api.FindAll().OrderBy(x => x.Title).ToList();

                await _cache.SetCustomValueAsync(FrameworksKey, MapFrom(frameworks));
            }

            return await _cache.GetCustomValueAsync<FrameworksView>(FrameworksKey);
        }

        public ProvidersView GetProvider(int ukPrn)
        {
            var api = new ProviderApiClient(_configuration.BaseUrl);

            return MapFrom(api.Get(ukPrn));
        }

        private static FrameworksView MapFrom(List<FrameworkSummary> frameworks)
        {
            return new FrameworksView
            {
                CreatedDate = DateTime.UtcNow,
                Frameworks = frameworks.Select(x => new Framework
                {
                    Id = int.Parse(x.Id),
                    Title = x.Title,
                    FrameworkCode = x.FrameworkCode,
                    FrameworkName = x.FrameworkName,
                    Level = x.Level,
                    PathwayCode = x.PathwayCode,
                    PathwayName = x.PathwayName,
                    Duration = new Duration
                    {
                        From = x.TypicalLength.From,
                        To = x.TypicalLength.To,
                        Unit = x.TypicalLength.Unit
                    }
                }).ToList()
            };
        }

        private static ProvidersView MapFrom(IEnumerable<Apprenticeships.Api.Types.Provider> providers)
        {
            return new ProvidersView
            {
                CreatedDate = DateTime.UtcNow,
                Providers = providers.Select(x => new Provider
                {
                    Ukprn = x.Ukprn,
                    ProviderName = x.ProviderName,
                    Email = x.Email,
                    Phone = x.Phone,
                    NationalProvider = x.NationalProvider
                }).ToList()
            };
        }

        private static StandardsView MapFrom(List<StandardSummary> standards)
        {
            return new StandardsView
            {
                CreationDate = DateTime.UtcNow,
                Standards = standards.Select(x => new EAS.Domain.Standard
                {
                    Id = int.Parse(x.Id),
                    Level = x.Level,
                    Title = x.Title,
                    Duration = new Duration
                    {
                        From = x.TypicalLength.From,
                        To = x.TypicalLength.To,
                        Unit = x.TypicalLength.Unit
                    }
                }).ToList()
            };
        }
    }
}