using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Client.Dtos;

namespace SFA.DAS.EAS.Account.Api.Client
{
    public class AccountApiClient : IAccountApiClient
    {
        private readonly AccountApiConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public AccountApiClient(AccountApiConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration);
        }
        internal AccountApiClient(AccountApiConfiguration configuration, SecureHttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<Dtos.PagedApiResponseViewModel<Dtos.AccountWithBalanceViewModel>> GetPageOfAccounts(int pageNumber = 1, int pageSize = 1000, DateTime? toDate = null)
        {
            var baseUrl = _configuration.ApiBaseUrl.EndsWith("/")
                ? _configuration.ApiBaseUrl
                : _configuration.ApiBaseUrl + "/";
            var url = $"{baseUrl}api/accounts?page={pageNumber}&pageSize={pageSize}";
            if (toDate.HasValue)
            {
                var formattedToDate = toDate.Value.ToString("yyyyMMdd");
                url += $"&toDate={formattedToDate}";
            }

            var json = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<Dtos.PagedApiResponseViewModel<Dtos.AccountWithBalanceViewModel>>(json);
        }

        public async Task<PagedApiResponseViewModel<AccountInformationViewModel>> GetPageOfAccountInformation(DateTime fromDate, DateTime toDate, int pageNumber = 1, int pageSize = 1000)
        {

            var fromDateFormatted = new DateTime(fromDate.Date.Year,fromDate.Month,fromDate.Day).ToString("yyyy-MM-dd");
            var toDateFormatted = new DateTime(toDate.Date.Year,toDate.Month,toDate.Day).ToString("yyyy-MM-dd");

            var baseUrl = _configuration.ApiBaseUrl.EndsWith("/")
                ? _configuration.ApiBaseUrl
                : _configuration.ApiBaseUrl + "/";
            var url = $"{baseUrl}api/accountsinformation?fromDate={fromDateFormatted}&toDate={toDateFormatted}&page={pageNumber}&pageSize={pageSize}";
            
            var json = await _httpClient.GetAsync(url);
            return JsonConvert.DeserializeObject<Dtos.PagedApiResponseViewModel<Dtos.AccountInformationViewModel>>(json);
        }
    }
}
