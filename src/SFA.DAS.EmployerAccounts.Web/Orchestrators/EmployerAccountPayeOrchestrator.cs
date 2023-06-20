using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EmployerAccounts.Queries.GetMember;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetTeamUser;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class EmployerAccountPayeOrchestrator : EmployerVerificationOrchestratorBase
{
    private readonly IEncodingService _encodingService;

    protected EmployerAccountPayeOrchestrator() { }

    public EmployerAccountPayeOrchestrator(
        IMediator mediator, 
        ICookieStorageService<EmployerAccountData> cookieService,
        EmployerAccountsConfiguration configuration,
        IEncodingService encodingService) : base(mediator, cookieService, configuration)
    {
        _encodingService = encodingService;
    }

    public async Task<OrchestratorResponse<EmployerAccountPayeListViewModel>> Get(string hashedAccountId, string externalUserId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

        var response = await Mediator.Send(new GetAccountPayeSchemesForAuthorisedUserQuery
        {
            AccountId = accountId,
            ExternalUserId = externalUserId
        });

        return new OrchestratorResponse<EmployerAccountPayeListViewModel>
        {
            Data = new EmployerAccountPayeListViewModel
            {
                PayeSchemes = response.PayeSchemes
            }
        };
    }

    public async Task<OrchestratorResponse<AddNewPayeSchemeViewModel>> GetPayeConfirmModel(string hashedId, string code, string redirectUrl, IQueryCollection queryCollection)
    {
        var response = await GetGatewayTokenResponse(code, redirectUrl, queryCollection);
        if (response.Status != HttpStatusCode.OK)
        {
            response.FlashMessage.ErrorMessages.Clear();
            response.FlashMessage.ErrorMessages.Add("addNewPaye", "Add new scheme");
            response.FlashMessage.Headline = "PAYE scheme not added";
            response.FlashMessage.Message = "You need to grant authority to HMRC to add a PAYE scheme.";

            return new OrchestratorResponse<AddNewPayeSchemeViewModel>
            {
                Data = new AddNewPayeSchemeViewModel
                {
                    HashedAccountId = hashedId
                },
                Status = response.Status,

                FlashMessage = response.FlashMessage,

            };
        }

        var hmrcResponse = await GetHmrcEmployerInformation(response.Data.AccessToken, string.Empty);

        return new OrchestratorResponse<AddNewPayeSchemeViewModel>
        {
            Data = new AddNewPayeSchemeViewModel
            {

                HashedAccountId = hashedId,
                PayeScheme = hmrcResponse.Empref,
                PayeName = hmrcResponse.EmployerLevyInformation?.Employer?.Name?.EmprefAssociatedName ?? "",
                EmprefNotFound = hmrcResponse.EmprefNotFound,
                AccessToken = !string.IsNullOrEmpty(hmrcResponse.Empref) ? response.Data.AccessToken : "",
                RefreshToken = !string.IsNullOrEmpty(hmrcResponse.Empref) ? response.Data.RefreshToken : ""
            }
        };

    }

    public async Task<OrchestratorResponse<GatewayInformViewModel>> CheckUserIsOwner(string hashedAccountId, string email, string redirectUrl, string confirmUrl)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var status = HttpStatusCode.OK;

        var response = await Mediator.Send(new GetMemberRequest
        {
            AccountId = accountId,
            Email = email,
            OnlyIfMemberIsActive = true
        });

        if (response != null && response.TeamMember.Role != Role.Owner)
        {
            status = HttpStatusCode.Unauthorized;
        }

        return new OrchestratorResponse<GatewayInformViewModel>
        {
            Data = new GatewayInformViewModel
            {
                ConfirmUrl = confirmUrl,
                BreadcrumbDescription = "Back to PAYE schemes"
            },
            Status = status,
            FlashMessage = new FlashMessageViewModel
            {
                RedirectButtonMessage = "Return to PAYE schemes"
            },
            RedirectUrl = redirectUrl
        };
    }


    public virtual async Task<OrchestratorResponse<AddNewPayeSchemeViewModel>> AddPayeSchemeToAccount(AddNewPayeSchemeViewModel model, string userId)
    {
        var response = new OrchestratorResponse<AddNewPayeSchemeViewModel> { Data = model };

        try
        {
            await Mediator.Send(new AddPayeToAccountCommand
            {
                HashedAccountId = model.HashedAccountId,
                AccessToken = model.AccessToken,
                RefreshToken = model.RefreshToken,
                Empref = model.PayeScheme,
                ExternalUserId = userId,
                EmprefName = model.PayeName
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            response.Status = HttpStatusCode.Unauthorized;
            response.Exception = ex;
        }
        catch (InvalidRequestException ex)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.Data.ErrorDictionary = ex.ErrorMessages;
            response.Exception = ex;
        }

        return response;
    }

    public virtual async Task<OrchestratorResponse<RemovePayeSchemeViewModel>> GetRemovePayeSchemeModel(RemovePayeSchemeViewModel model)
    {
        var accountId = _encodingService.Decode(model.HashedAccountId, EncodingType.AccountId);

        var accountResponse = await
            Mediator.Send(new GetEmployerAccountByIdQuery
            {
                AccountId = accountId,
                UserId = model.UserId
            });

        var payeResponse = await
            Mediator.Send(new GetPayeSchemeByRefQuery
            {
                HashedAccountId = model.HashedAccountId,
                Ref = model.PayeRef
            });

        model.AccountName = accountResponse.Account.Name;
        model.PayeSchemeName = payeResponse.PayeScheme.Name;

        return new OrchestratorResponse<RemovePayeSchemeViewModel> { Data = model };
    }

    //TODO: The message gets mutated by the method. Message needs to be made immutable
    public virtual async Task<OrchestratorResponse<RemovePayeSchemeViewModel>> RemoveSchemeFromAccount(RemovePayeSchemeViewModel model)
    {
        var response = new OrchestratorResponse<RemovePayeSchemeViewModel> { Data = model };
        try
        {
            var result = await Mediator.Send(new GetPayeSchemeByRefQuery
            {
                HashedAccountId = model.HashedAccountId,
                Ref = model.PayeRef,
            });

            model.PayeSchemeName = result.PayeScheme.Name;

            await Mediator.Send(new RemovePayeFromAccountCommand(model.HashedAccountId,
                model.PayeRef, model.UserId, model.RemoveScheme == 2, model.PayeSchemeName));


            response.Data = model;
        }
        catch (UnauthorizedAccessException)
        {
            response.Status = HttpStatusCode.Unauthorized;
        }
        catch (InvalidRequestException ex)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.Data.ErrorDictionary = ex.ErrorMessages;
        }

        return response;
    }

    public async Task<OrchestratorResponse<PayeSchemeDetailViewModel>> GetPayeDetails(string empRef, string hashedAccountId, string userId)
    {
        var response = new OrchestratorResponse<PayeSchemeDetailViewModel>();
        try
        {
            var englishFractionResult = await Mediator.Send(new GetEmployerEnglishFractionHistoryQuery
            {
                HashedAccountId = hashedAccountId,
                EmpRef = empRef,
                UserId = userId
            });

            var payeSchemeResult = await Mediator.Send(new GetPayeSchemeByRefQuery
            {
                HashedAccountId = hashedAccountId,
                Ref = empRef
            });

            response.Data = new PayeSchemeDetailViewModel
            {
                Fractions = englishFractionResult.Fractions,
                EmpRef = englishFractionResult.EmpRef,
                PayeSchemeName = payeSchemeResult?.PayeScheme?.Name ?? string.Empty,
                EmpRefAdded = englishFractionResult.EmpRefAddedDate
            };
            return response;
        }
        catch (UnauthorizedAccessException)
        {
            response.Status = HttpStatusCode.Unauthorized;
        }

        return response;
    }

    public async Task<OrchestratorResponse<PayeSchemeNextStepsViewModel>> GetNextStepsViewModel(string userId, string hashedAccountId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var response = new OrchestratorResponse<PayeSchemeNextStepsViewModel>();

        var userResponse = await Mediator.Send(new GetTeamMemberQuery { AccountId = accountId, TeamMemberId = userId });
        var showWizard = userResponse.User.ShowWizard && userResponse.User.Role == Role.Owner;
        response.Data = new PayeSchemeNextStepsViewModel
        {
            ShowWizard = showWizard
        };

        return response;
    }
}