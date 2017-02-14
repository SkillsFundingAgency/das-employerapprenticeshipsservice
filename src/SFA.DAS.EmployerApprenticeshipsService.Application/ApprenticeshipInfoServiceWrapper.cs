using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EAS.Domain.Models.Time;
using Framework = SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse.Framework;
using Provider = SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider.Provider;
using Standard = SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse.Standard;

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
            try
            {
                var api = new ProviderApiClient(_configuration.BaseUrl);
                var response = api.Get(ukPrn);
                var providersView = MapFrom(response);

                return providersView;
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Apprenticeships.Api.Types.Exceptions.EntityNotFoundException)
            {
                return new ProvidersView
                {
                    CreatedDate = DateTime.UtcNow,
                    Providers = new List<Provider>()
                };
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
                    Duration = x.TypicalLength  == null ? null : new Duration // TODO: LWA - Should frameworks have a null typical length?
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
                Standards = standards.Select(x => new Standard
                {
                    Id = x.Id,
                    Code = long.Parse(x.Id),
                    Level = x.Level,
                    Title = GetTitle(x.Title, x.Level),
                    Duration = new Duration
                    {
                        From = x.TypicalLength.From,
                        To = x.TypicalLength.To,
                        Unit = x.TypicalLength.Unit
                    }
                }).ToList()
            };
        }

        private static string GetTitle(string title, int level)
        {
            return $"{title}, Level: {level}";
        }
    }
}