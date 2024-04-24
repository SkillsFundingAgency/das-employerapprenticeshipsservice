﻿using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Authorization;
using SFA.DAS.EAS.Support.Web.Configuration;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.EAS.Support.Web.Services;
using AccountDetailViewModel = SFA.DAS.EAS.Support.Web.Models.AccountDetailViewModel;

namespace SFA.DAS.EAS.Support.Web.Controllers;

[Authorize(Policy = PolicyNames.Default)]
public class AccountController : Controller
{
    private readonly IEasSupportConfiguration _easSupportConfiguration;
    private readonly IAccountHandler _accountHandler;
    private readonly IPayeLevySubmissionsHandler _payeLevySubmissionsHandler;
    private readonly IPayeLevyMapper _payeLevyMapper;

    public AccountController(
        IEasSupportConfiguration easSupportConfiguration,
        IAccountHandler accountHandler,
        IPayeLevySubmissionsHandler payeLevySubmissionsHandler,
        IPayeLevyMapper payeLevyDeclarationMapper)
    {
        _easSupportConfiguration = easSupportConfiguration;
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
            IsTier2User = User.IsInRole(AuthorizationConstants.Tier2User),
            TeamMemberUrl = GetTeamMemberUrl(id),
            ChangeRoleUrl = $"/resource/index/{{0}}/?childId={{1}}key={SupportServiceResourceKey.EmployerAccountChangeRole}",
            ResendInviteUrl = $"/resource/index/{{0}}/?childId={{1}}key={SupportServiceResourceKey.EmployerAccountResendInvitation}"
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

    [HttpGet]
    [Route("account/{id}change-role/{userRef}")]
    public async Task<IActionResult> ChangeRole(string id, string userRef)
    {
        var accountResponse = await _accountHandler.FindTeamMembers(id);

        if (accountResponse.StatusCode == SearchResponseCodes.NoSearchResultsFound)
        {
            return NotFound();
        }

        var teamMember = accountResponse.Account.TeamMembers.Single(x => x.UserRef == userRef);

        return View(new ChangeRoleViewModel
        {
            HashedAccountId = accountResponse.Account.HashedAccountId,
            TeamMemberUserRef = teamMember.UserRef,
            Role = Enum.Parse<Role>(teamMember.Role)
        });
    }

    [HttpPost]
    [Route("account/{id}change-role/{userRef}")]
    public async Task<IActionResult> ChangeRole(string id, string userRef, Role role)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("account/{id}/resend-invitation/{userRef}")]
    public async Task<IActionResult> ResendInvitation(string id, string userRef)
    {
        throw new NotImplementedException();
    }

    private string GetTeamMemberUrl(string hashedAccountId)
    {
        var baseUrl = _easSupportConfiguration.EmployerAccountsConfiguration.EmployerAccountsBaseUrl;
        var trimmedBaseUrl = baseUrl?.TrimEnd('/') ?? string.Empty;
        string path = $"login/staff?HashedAccountId={hashedAccountId}";

        return $"{trimmedBaseUrl}/{path}".TrimEnd('/');
    }
}