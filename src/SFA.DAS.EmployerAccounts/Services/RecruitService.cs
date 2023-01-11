using AutoMapper;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Services;

public class RecruitService : IRecruitService
{
    private readonly EmployerAccountsConfiguration _configuration;
    private readonly IHttpService _httpService;
    private readonly IMapper _mapper;

    public RecruitService(
        IHttpServiceFactory httpServiceFactory,
        EmployerAccountsConfiguration configuration,
        IMapper mapper)
    {
        _configuration = configuration;
        _httpService = httpServiceFactory.Create(configuration.RecruitApi.IdentifierUri);
        _mapper = mapper;
    }

    public async Task<IEnumerable<Vacancy>> GetVacancies(string hashedAccountId, int maxVacanciesToGet = int.MaxValue)
    {
        var baseUrl = GetBaseUrl();
        var url = $"{baseUrl}api/vacancies?employerAccountId={hashedAccountId}&pageSize={maxVacanciesToGet}";

        var json = await _httpService.GetAsync(url, false);
        if(json == null)
        {
            return new List<Vacancy>();
        }

        var vacanciesSummary = JsonConvert.DeserializeObject<VacanciesSummary>(json);


        return _mapper.Map<IEnumerable<VacancySummary>, IEnumerable<Vacancy>>(vacanciesSummary.Vacancies);
    }

    private string GetBaseUrl()
    {
        var baseUrl = _configuration.RecruitApi.ApiBaseUrl.EndsWith("/")
            ? _configuration.RecruitApi.ApiBaseUrl
            : _configuration.RecruitApi.ApiBaseUrl + "/";

        return baseUrl;
    }
}