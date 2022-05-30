using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Authentication.Extensions.Legacy;
using System.Net.Http;
using System.Configuration;
using Microsoft.Azure.Services.AppAuthentication;
using System.Net.Http.Headers;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class RecruitService : ApiClientBase, IRecruitService
    {   
        private readonly IMapper _mapper;
        private readonly HttpClient _client;
        private readonly string _apiBaseUrl;
        private readonly string _identifierUri;

        public RecruitService(
            HttpClient client,            
            IRecruitClientApiConfiguration configuration,
            IMapper mapper) : base(client)
        {
            _apiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
              ? configuration.ApiBaseUrl
              : configuration.ApiBaseUrl + "/";

            _identifierUri = configuration.IdentifierUri;
            _client = client;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Vacancy>> GetVacancies(string hashedAccountId, int maxVacanciesToGet = int.MaxValue)
        {
            await AddAuthenticationHeader();

            var url = $"{_apiBaseUrl}api/vacancies?employerAccountId={hashedAccountId}&pageSize={maxVacanciesToGet}";

            var json = await _client.GetAsync(url);
            if(json == null)
            {
                return new List<Vacancy>();
            }
            string jsonString = json.Content.ReadAsStringAsync().Result;           

            var vacanciesSummary = JsonConvert.DeserializeObject<VacanciesSummary>(jsonString);

            return _mapper.Map<IEnumerable<VacancySummary>, IEnumerable<Vacancy>>(vacanciesSummary.Vacancies);
        }

        private async Task AddAuthenticationHeader()
        {
            if (ConfigurationManager.AppSettings["EnvironmentName"].ToUpper() != "LOCAL")
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(_identifierUri);

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }
    }
}
