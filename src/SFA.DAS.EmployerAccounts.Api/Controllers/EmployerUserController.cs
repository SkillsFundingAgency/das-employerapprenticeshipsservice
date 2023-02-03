using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[Route("api/user/{userRef}")]
public class EmployerUserController : ControllerBase
{
    private readonly UsersOrchestrator _orchestrator;

    public EmployerUserController(UsersOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [Route("accounts", Name = "Accounts")]
    [DasAuthorize(Roles = "ReadUserAccounts")]
    [HttpGet]
    public async Task<IActionResult> GetUserAccounts(string userRef)
    {
        return Ok(await _orchestrator.GetUserAccounts(userRef));
    }
}