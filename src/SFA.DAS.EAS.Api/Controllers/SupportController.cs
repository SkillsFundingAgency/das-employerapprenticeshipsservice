using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.EAS.Account.Api.Authorization;
using SFA.DAS.EAS.Account.Api.Requests;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers;

[ApiController]
[Route("api/support")]
[Authorize(Policy = ApiRoles.ReadUserAccounts)]
public class SupportController : ControllerBase
{
    private readonly IEmployerAccountsApiService _apiService;
    private readonly ILogger<SupportController> _logger;

    public SupportController(IEmployerAccountsApiService apiService, ILogger<SupportController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [HttpPost]
    [Route("change-role")]
    public async Task<IActionResult> ChangeRole([FromBody] ChangeTeamMemberRoleRequest request)
    {
        try
        {
            await _apiService.RedirectPost("/api/support/change-role", JsonConvert.SerializeObject(request), CancellationToken.None);
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error in {Controller}.{Action}", nameof(SupportController), nameof(ChangeRole));
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost]
    [Route("resend-invitation")]
    public async Task<IActionResult> ResendInvitation([FromBody] ResendInvitationRequest request)
    {
        _logger.LogWarning("{Controller}.{Action} request: {Request}", nameof(SupportController), nameof(ResendInvitation), JsonConvert.SerializeObject(request));
        
        try
        {
            await _apiService.RedirectPost("/api/support/resend-invitation", request, CancellationToken.None);
            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error in {Controller}.{Action}", nameof(SupportController), nameof(ResendInvitation));
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}