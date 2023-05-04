using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Authorize(Policy = "LoopBack", Roles = ApiRoles.ReadUserAccounts)]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IEmployerAccountsApiService _apiService;

    public UserController(IEmployerAccountsApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string email)
    {
        try
        {
            return Ok(await _apiService.Redirect($"/api/user?email={email}"));
        }
        catch
        {
            return NotFound();
        }
    }
}
