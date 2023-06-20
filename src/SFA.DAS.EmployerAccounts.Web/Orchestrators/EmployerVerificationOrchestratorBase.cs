using System.Data;
using SFA.DAS.EmployerAccounts.Queries.GetGatewayInformation;
using SFA.DAS.EmployerAccounts.Queries.GetGatewayToken;
using SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;


namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public abstract class EmployerVerificationOrchestratorBase
{
    protected readonly IMediator Mediator;
    protected readonly ICookieStorageService<EmployerAccountData> CookieService;
    protected readonly EmployerAccountsConfiguration Configuration;

    //Needed for tests
    protected EmployerVerificationOrchestratorBase() { }

    protected EmployerVerificationOrchestratorBase(IMediator mediator, ICookieStorageService<EmployerAccountData> cookieService, EmployerAccountsConfiguration configuration)
    {
        Mediator = mediator;
        CookieService = cookieService;
        Configuration = configuration;
    }


    public virtual Task<GetUserAccountRoleResponse> GetUserAccountRole(long accountId, string externalUserId)
    {
        return Mediator.Send(new GetUserAccountRoleQuery
        {
            AccountId = accountId,
            ExternalUserId = externalUserId
        });
    }

    public virtual async Task<string> GetGatewayUrl(string redirectUrl)
    {
        var response = await Mediator.Send(new GetGatewayInformationQuery
        {
            ReturnUrl = redirectUrl
        });

        return response.Url;
    }

    public async Task<OrchestratorResponse<HmrcTokenResponse>> GetGatewayTokenResponse(string accessCode, string returnUrl, IQueryCollection queryCollection)
    {
        var errorResponse = queryCollection?["error"].ToString();
        if (!string.IsNullOrEmpty(errorResponse))
        {
            if (queryCollection["error_Code"].ToString() == "USER_DENIED_AUTHORIZATION")
            {
                return new OrchestratorResponse<HmrcTokenResponse>
                {
                    Status = HttpStatusCode.NotAcceptable,
                    FlashMessage = new FlashMessageViewModel
                    {
                        Severity = FlashMessageSeverityLevel.Error,
                        Headline = "Account not added",
                        Message = "You need to grant authority to HMRC to add an account.",
                        ErrorMessages = new Dictionary<string, string> { { "agree_and_continue", "Agree and continue" } }
                    }
                };
            }

            return new OrchestratorResponse<HmrcTokenResponse>
            {
                Status = HttpStatusCode.NotAcceptable,
                FlashMessage = new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Danger,
                    Message = "Unexpected response from HMRC Government Gateway:",
                    SubMessage = queryCollection["error_description"]
                }
            };
        }

        var response = await Mediator.Send(new GetGatewayTokenQuery
        {
            RedirectUrl = returnUrl,
            AccessCode = accessCode
        });

        return new OrchestratorResponse<HmrcTokenResponse> { Data = response.HmrcTokenResponse };
    }


    public async Task<GetHmrcEmployerInformationResponse> GetHmrcEmployerInformation(string authToken, string email)
    {
        var response = new GetHmrcEmployerInformationResponse();
        try
        {
            response = await Mediator.Send(new GetHmrcEmployerInformationQuery
            {
                AuthToken = authToken
            });
        }
        catch (ConstraintException)
        {
            response.Empref = "";
        }
        catch (NotFoundException)
        {
            response.Empref = "";
            response.EmprefNotFound = true;
        }

        return response;
    }
}