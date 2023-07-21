using System.Net;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;

namespace SFA.DAS.EmployerAccounts.Services;

public class PensionRegulatorService : IPensionRegulatorService
{
    private readonly EmployerAccountsConfiguration _configuration;
    private readonly IHttpService _httpService;    
       
    public PensionRegulatorService(
        IHttpServiceFactory httpServiceFactory,
        EmployerAccountsConfiguration configuration)
    {           
        _configuration = configuration;         
        _httpService = httpServiceFactory.Create(
            configuration.PensionRegulatorApi.IdentifierUri,
            configuration.PensionRegulatorApi.ClientId,
            configuration.PensionRegulatorApi.ClientSecret,                
            configuration.PensionRegulatorApi.Tenant
        );
    }

    public async Task<Organisation> GetOrganisationById(string organisationId)
    {
        var baseUrl = GetBaseUrl();
        var url = $"{baseUrl}api/pensionsregulator/{organisationId}";

        var json = await _httpService.GetAsync(url, false);
        return json == null ? null : JsonConvert.DeserializeObject<Organisation>(json);
    }

    public async Task<IEnumerable<Organisation>> GetOrganisationsByPayeRef(string payeRef)
    {
        var baseUrl = GetBaseUrl();
        var url = $"{baseUrl}api/pensionsregulator/organisations?payeRef={WebUtility.UrlEncode(payeRef)}";

        var json = await _httpService.GetAsync(url, false);
        return json == null ? null : JsonConvert.DeserializeObject<IEnumerable<Organisation>>(json);
    }

    public async Task<IEnumerable<Organisation>> GetOrganisationsByAorn(string aorn, string payeRef)
    {
        var baseUrl = GetBaseUrl();
        var url = $"{baseUrl}api/pensionsregulator/organisations/{WebUtility.UrlEncode(aorn)}?payeRef={WebUtility.UrlEncode(payeRef)}";

        var json = await _httpService.GetAsync(url, false);
        return json == null ? null : JsonConvert.DeserializeObject<IEnumerable<Organisation>>(json);
    }

    private string GetBaseUrl()
    {
        var baseUrl = _configuration.PensionRegulatorApi.BaseUrl.EndsWith("/")
            ? _configuration.PensionRegulatorApi.BaseUrl
            : _configuration.PensionRegulatorApi.BaseUrl + "/";

        return baseUrl;
    }
}