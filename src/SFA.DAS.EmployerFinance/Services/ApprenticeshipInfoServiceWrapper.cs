using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Caches;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.TrainingCourses;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.TrainingCourses;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerFinance.Services
{
    public class ApprenticeshipInfoServiceWrapper : IApprenticeshipInfoServiceWrapper
    {
        private const string StandardsKey = "Standards";
        private const string FrameworksKey = "Frameworks";

        private readonly IInProcessCache _cache;
        private readonly IOuterApiClient _outerApiClient;

        public ApprenticeshipInfoServiceWrapper(IInProcessCache cache, IOuterApiClient apiClient)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _outerApiClient = apiClient;
        }

        public async Task<StandardsView> GetStandardsAsync(bool refreshCache = false)
        {
            if (!_cache.Exists(StandardsKey) || refreshCache)
            {
                var response = await _outerApiClient.Get<GetStandardsResponse>(new GetStandardsRequest());

                _cache.Set(StandardsKey, MapFrom(response.Standards));
            }

            return _cache.Get<StandardsView>(StandardsKey);
        }

        public async Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false)
        {
            if (!_cache.Exists(FrameworksKey) || refreshCache)
            {
                var response = await _outerApiClient.Get<GetFrameworksResponse>(new GetFrameworksRequest());

                _cache.Set(FrameworksKey, MapFrom(response.Frameworks));
            }

            return _cache.Get<FrameworksView>(FrameworksKey);
        }       

        private static FrameworksView MapFrom(List<FrameworkResponse> frameworks)
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

        private static StandardsView MapFrom(List<StandardResponse> standards)
        {
            return new StandardsView
            {
                CreationDate = DateTime.UtcNow,
                Standards = standards.Select(x => new Standard
                {
                    Id = x.Id.ToString(),
                    Code = x.Id,
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