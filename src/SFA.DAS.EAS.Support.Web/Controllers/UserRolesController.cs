using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Route("roles")]
[AllowAnonymous]
//[Authorize(Policy = PolicyNames.Default)]
public class UserRolesController(IAccountHandler accountHandler, ILogger<UserRolesController> logger) : Controller
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

    [HttpPost]
    [Route("{id}/{userRef}")]
    public IActionResult IndexPost(string id, string userRef)
    {
        logger.LogInformation("Roles controller, POST ChangeRole. AccountId: {AccountId}. UserRef: {UserRef}. UpdatedRole: MISSING", id, userRef);
        //logger.LogInformation("Roles controller, POST ChangeRole. AccountId: {AccountId}. UserRef: {UserRef}. UpdatedRole: {Role}", id, userRef, (Role)role);

        return Redirect(string.Format($"/resource/index/{{0}}/?key={SupportServiceResourceKey.EmployerAccountTeam}", id));
    }
}