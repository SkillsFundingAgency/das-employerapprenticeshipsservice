using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Authorize(Policy = "LoopBack", Roles = ApiRoles.ReadUserAccounts)]
[Route("api/accounts/{hashedAccountId}/transfers/connections")]
public class TransferConnectionsController : ControllerBase
{
    private readonly IEmployerFinanceApiService _apiService;

    public TransferConnectionsController(IEmployerFinanceApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransferConnections(string hashedAccountId)
    {
        return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/transfers/connections"));
    }
}