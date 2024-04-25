using Opw.HttpExceptions;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Route("role/{id}")]
[Authorize(Policy = PolicyNames.Default)]
public class RoleController(IAccountHandler accountHandler, ILogger<RoleController> logger) : Controller
{
    [HttpGet]
    public async Task<ActionResult> Header(string id, string userRef)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new BadRequestException();

        var response = await accountHandler.FindOrganisations(id);

        if (response == null)
        {
            return NotFound();
        }

        var teamMember = response.Account.TeamMembers.Single(x => x.UserRef == userRef);

        return View("SubHeader", new ChangeRoleViewModel { Name = teamMember.Name });
    }

    [HttpGet]
    [Route("{userRef}")]
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
            PostbackUrl = string.Format($"/resource/index/{{0}}/?childId={{1}}&key={SupportServiceResourceKey.EmployerAccountChangeRole}", id, userRef),
        });
    }

    [HttpPost]
    [Route("{userRef}")]
    public IActionResult Index(string id, string userRef, Role role)
    {
        logger.LogInformation("Role controller, POST ChangeRole. AccountId: {AccountId}. UserRef: {UserRef}. UpdatedRole: {Role}", id, userRef, role);

        return Redirect(string.Format($"/resource/index/{{0}}/?key={SupportServiceResourceKey.EmployerAccountTeam}", id));
    }
}