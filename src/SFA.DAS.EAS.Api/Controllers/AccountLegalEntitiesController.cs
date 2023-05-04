using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Authorize(Policy = "LoopBack", Roles = ApiRoles.ReadUserAccounts)]
[Route("api/accountlegalentities")]
public class AccountLegalEntitiesController : ControllerBase
{      
    private readonly IEmployerAccountsApiService _apiService;

    public AccountLegalEntitiesController(IEmployerAccountsApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int? pageSize, int? pageNumber)
    {
        return Ok(await _apiService.Redirect($"/api/accountlegalentities?{(pageSize.HasValue ? "pageSize=" + pageSize + "&" : "")}{(pageNumber.HasValue ? "pageNumber=" + pageNumber : "")}"));
    }
}
