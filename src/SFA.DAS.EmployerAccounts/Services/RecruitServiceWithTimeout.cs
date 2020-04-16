using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using SFA.DAS.EmployerAccounts.Exceptions;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class RecruitServiceWithTimeout : IRecruitService
    {
        private readonly EmployerAccountsConfiguration _configuration;
        private readonly IHttpService _httpService;
        private readonly IMapper _mapper;
        private readonly IAsyncPolicy _pollyPolicy;

        public RecruitServiceWithTimeout(
            IHttpServiceFactory httpServiceFactory,
            EmployerAccountsConfiguration configuration,
            IMapper mapper, IReadOnlyPolicyRegistry<string> pollyRegistry)
        {
            _configuration = configuration;
            _httpService = httpServiceFactory.Create(
                configuration.RecruitApi.ClientId,
                configuration.RecruitApi.ClientSecret,
                configuration.RecruitApi.IdentifierUri,
                configuration.RecruitApi.Tenant
            );
            _mapper = mapper;
            _pollyPolicy = pollyRegistry.Get<IAsyncPolicy>(Constants.DefaultServiceTimeout);
        }

        public async Task<IEnumerable<Vacancy>> GetVacancies(string hashedAccountId, int maxVacanciesToGet = int.MaxValue)
        {
            try
            {
                var baseUrl = GetBaseUrl();
                var url = $"{baseUrl}api/vacancies?employerAccountId={hashedAccountId}&pageSize={maxVacanciesToGet}";
                
                var json = await _pollyPolicy.ExecuteAsync(() => _httpService.GetAsync(url, false));
                
                if (json == null)
                {
                    return new List<Vacancy>();
                }

                var vacanciesSummary = JsonConvert.DeserializeObject<VacanciesSummary>(json);
                return _mapper.Map<IEnumerable<VacancySummary>, IEnumerable<Vacancy>>(vacanciesSummary.Vacancies);
            }
            catch (TimeoutRejectedException ex)
            {
                throw new ServiceTimeoutException("Call to Recruit Service timed out", ex);
            }
        }

        private string GetBaseUrl()
        {
            var baseUrl = _configuration.RecruitApi.ApiBaseUrl.EndsWith("/")
                ? _configuration.RecruitApi.ApiBaseUrl
                : _configuration.RecruitApi.ApiBaseUrl + "/";

            return baseUrl;
        }
    }
}
