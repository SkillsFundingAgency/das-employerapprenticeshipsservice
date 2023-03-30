using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiController]
    [Route("api/user/{userRef}")]
    public class EmployerUserController : ControllerBase
    {
        private readonly IEmployerAccountsApiService _apiService;

        public EmployerUserController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Authorize(Policy = "LoopBack", Roles = "ReadUserAccounts")]
        [HttpGet("accounts", Name = "Accounts")]
        public async Task<IActionResult> GetUserAccounts(string userRef)
        {
            return Ok(await _apiService.Redirect($"/api/user/{userRef}/accounts"));
        }
    }
}