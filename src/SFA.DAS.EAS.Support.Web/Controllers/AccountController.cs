using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Helpers;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.EAS.Support.Web.Services;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Authorize(Policy = PolicyNames.Default)]
public class AccountController : Controller
{
    private readonly IAccountHandler _accountHandler;
    private readonly IPayeLevySubmissionsHandler _payeLevySubmissionsHandler;
    private readonly IPayeLevyMapper _payeLevyMapper;

    public AccountController(IAccountHandler accountHandler,
        IPayeLevySubmissionsHandler payeLevySubmissionsHandler,
        IPayeLevyMapper payeLevyDeclarationMapper)
    {
        _accountHandler = accountHandler;
        _payeLevySubmissionsHandler = payeLevySubmissionsHandler;
        _payeLevyMapper = payeLevyDeclarationMapper;
    }

    [Route("account/{id}")]
    public async Task<IActionResult> Index(string id)
    {
        var response = await _accountHandler.FindOrganisations(id);

        if (response.StatusCode != SearchResponseCodes.Success)
        {
            return NotFound();
        }
        
        var model = new AccountDetailViewModel
        {
            Account = response.Account,
            AccountUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerUser}"

        };

        return View(model);
    }

    [Route("account/payeschemes/{id}")]
    public async Task<IActionResult> PayeSchemes(string id)
    {
        var response = await _accountHandler.FindPayeSchemes(id);

        if (response.StatusCode != SearchResponseCodes.Success)
        {
            return new NotFoundResult();
        }
        
        var model = new AccountDetailViewModel
        {
            Account = response.Account,
            AccountUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerUser}"
        };

        return View(model);
    }

    [Route("account/header/{id}")]
    public async Task<IActionResult> Header(string id)
    {
        var response = await _accountHandler.Find(id);

        if (response.StatusCode != SearchResponseCodes.Success)
        {
            return NotFound();
        }

        return View("SubHeader", response.Account);
    }

    [Route("account/team/{id}")]
    public async Task<IActionResult> Team(string id)
    {
        var response = await _accountHandler.FindTeamMembers(id);

        if (response.StatusCode != SearchResponseCodes.Success)
        {
            return NotFound();
        }
        
        var model = new AccountDetailViewModel
        {
            Account = response.Account,
            AccountUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerUser}",
            IsTier2User = User.IsInRole(AuthorizationConstants.Tier2User)
        };

        return View(model);

    }

    [Route("account/finance/{id}")]
    public async Task<IActionResult> Finance(string id)
    {
        var response = await _accountHandler.FindFinance(id);

        if (response.StatusCode != SearchResponseCodes.Success)
        {
            return NotFound();
        }
        
        var model = new FinanceViewModel
        {
            Account = response.Account,
            Balance = response.Balance
        };

        return View(model);
    }

    [Route("account/levysubmissions/{id}/{payeSchemeId}")]
    public async Task<IActionResult> PayeSchemeLevySubmissions(string id, string payeSchemeId)
    {
        var response = await _payeLevySubmissionsHandler.FindPayeSchemeLevySubmissions(id, payeSchemeId);

        if (response.StatusCode == PayeLevySubmissionsResponseCodes.AccountNotFound)
        {
            return NotFound();
        }
        
        var model = _payeLevyMapper.MapPayeLevyDeclaration(response);

        model.UnexpectedError = response.StatusCode == PayeLevySubmissionsResponseCodes.UnexpectedError;

        return View(model);
    }
}