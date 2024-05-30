using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Route("roles")]
[Authorize(Policy = PolicyNames.Default)]
public class UserRolesController(IAccountHandler accountHandler, ILogger<UserRolesController> logger, IEmployerAccountsApiService accountsApiService) : Controller
{
    [HttpGet]
    [Route("{id}/{userRef}")]
    public async Task<IActionResult> Index(string id, string userRef)
    {
        var accountResponse = await accountHandler.FindTeamMembers(id);

        if (accountResponse.StatusCode == SearchResponseCodes.NoSearchResultsFound)
        {
            return NotFound();
        }

        var teamMember = accountResponse.Account.TeamMembers.Single(x => x.UserRef == userRef);

        return View(new ChangeRoleViewModel
        {
            HashedAccountId = accountResponse.Account.HashedAccountId,
            UserRef = teamMember.UserRef,
            Role = Enum.Parse<Role>(teamMember.Role),
            Name = teamMember.Name,
            ResponseUrl = $"/resource/role/change/{id}/{userRef}",
        });
    }

    // public record UpdateRoleRequest
    // {
    //     public int Role { get; set; }
    //     public string SupportUserEmail { get; set; }
    // }
    //
    // [HttpPost]
    // [Route("{id}/{userRef}")]
    // public async Task<IActionResult> Update(string id, string userRef, [FromBody] UpdateRoleRequest request)
    // {
    //     var model = new ChangeRoleCompletedModel
    //     {
    //         ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id),
    //         Success = true,
    //         Role = request.Role
    //     };
    //
    //     try
    //     {
    //         var accountResponse = await accountHandler.FindTeamMembers(id);
    //
    //         var teamMember = accountResponse.Account.TeamMembers.Single(x => x.UserRef == userRef);
    //         model.MemberEmail = teamMember.Email;
    //
    //         await accountsApiService.ChangeRole(id, teamMember.Email, request.Role, request.SupportUserEmail);
    //     }
    //     catch (Exception exception)
    //     {
    //         logger.LogError(exception, $"{nameof(UserRolesController)}.{nameof(Update)} caught exception.");
    //         model.Success = false;
    //     }
    //
    //     return View("Confirm", model);
    // }

    [Route("Confirm/{id}/{userRef}")]
    public async Task<IActionResult> Confirm(string id, string userRef)
    {
        var accountResponse = await accountHandler.FindTeamMembers(id);

        var teamMember = accountResponse.Account.TeamMembers.Single(x => x.UserRef == userRef);
        var newRole = Enum.Parse<Role>(teamMember.Role);
        
        var model = new ChangeRoleCompletedModel
        {
            ReturnToTeamUrl = string.Format($"/resource?key={SupportServiceResourceKey.EmployerAccountTeam}&id={{0}}", id),
            Success = true,
            Role = (int)newRole,
            MemberEmail = teamMember.Email
        };
        
        return View("Confirm", model);
    }
}