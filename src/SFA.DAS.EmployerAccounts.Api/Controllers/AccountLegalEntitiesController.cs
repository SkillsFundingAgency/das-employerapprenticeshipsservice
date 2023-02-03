using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Api.Authorization;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;

namespace SFA.DAS.EmployerAccounts.Api.Controllers;

[DasAuthorize(Roles = ApiRoles.ReadUserAccounts)]
[Route("api/accountlegalentities")]
public class AccountLegalEntitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountLegalEntitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAccountLegalEntitiesQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response.AccountLegalEntities);
    }
}