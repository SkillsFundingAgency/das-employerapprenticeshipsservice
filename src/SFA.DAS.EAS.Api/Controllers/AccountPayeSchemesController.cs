using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiController]
    [Route("api/accounts/{hashedAccountId}/payeschemes")]
    public class AccountPayeSchemesController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly IEmployerAccountsApiService _apiService;

        public AccountPayeSchemesController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Authorize(Policy = "LoopBack", Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet(Name = "GetPayeSchemes")]
        public async Task<IActionResult> GetPayeSchemes(string hashedAccountId)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/payeschemes"));
        }

        [Authorize(Policy = "LoopBack", Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet("{payeschemeref}", Name = "GetPayeScheme")]
        public async Task<IActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/payeschemes/{WebUtility.UrlEncode(payeSchemeRef)}"));
        }
    }
}