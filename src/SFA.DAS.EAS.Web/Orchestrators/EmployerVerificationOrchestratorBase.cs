using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetGatewayInformation;
using SFA.DAS.EAS.Application.Queries.GetGatewayToken;
using SFA.DAS.EAS.Application.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public abstract class EmployerVerificationOrchestratorBase
    {
        protected readonly IMediator Mediator;
        protected readonly ILogger Logger;
        protected readonly ICookieService<EmployerAccountData> CookieService;
        protected readonly EmployerApprenticeshipsServiceConfiguration Configuration;

        //Needed for tests
        protected EmployerVerificationOrchestratorBase()
        {

        }

        protected EmployerVerificationOrchestratorBase(IMediator mediator, ILogger logger, ICookieService<EmployerAccountData> cookieService, EmployerApprenticeshipsServiceConfiguration configuration)
        {
            Mediator = mediator;
            Logger = logger;
            CookieService = cookieService;
            Configuration = configuration;
        }


        public virtual async Task<string> GetGatewayUrl(string redirectUrl)
        {
            var response = await Mediator.SendAsync(new GetGatewayInformationQuery
            {
                ReturnUrl = redirectUrl
            });

            return response.Url;
        }

        public async Task<OrchestratorResponse<HmrcTokenResponse>> GetGatewayTokenResponse(string accessCode, string returnUrl, NameValueCollection nameValueCollection)
        {
            var errorResponse = nameValueCollection?["error"];
            if (errorResponse != null)
            {
                if (nameValueCollection["error_Code"] == "USER_DENIED_AUTHORIZATION" )
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
                        SubMessage = nameValueCollection["error_description"]
                    }
                };
            }

            var response = await Mediator.SendAsync(new GetGatewayTokenQuery
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
                response = await Mediator.SendAsync(new GetHmrcEmployerInformationQuery
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

        public virtual async Task<OrchestratorResponse<OrganisationDetailsViewModel>> GetCompanyDetails(SelectEmployerViewModel viewModel)
        {
            var response = await Mediator.SendAsync(new GetEmployerInformationRequest
            {
                Id = viewModel.EmployerRef
            });

            if (response == null)
            {
                Logger.Warn("No response from SelectEmployerViewModel");
                return new OrchestratorResponse<OrganisationDetailsViewModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    Data = new OrganisationDetailsViewModel()
                };
            }

            Logger.Info($"Returning Data for {viewModel.EmployerRef}");

            return new OrchestratorResponse<OrganisationDetailsViewModel>
            {
                Data = new OrganisationDetailsViewModel
                {
                    ReferenceNumber = response.CompanyNumber,
                    Name = response.CompanyName,
                    DateOfInception = response.DateOfIncorporation,
                    Address = $"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}",
                    Status = response.CompanyStatus,
                }

            };
        }

        public virtual async Task<GetUserAccountRoleResponse> GetUserAccountRole(string hashedAccountId, string externalUserId)
        {
            return await Mediator.SendAsync(new GetUserAccountRoleQuery
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId
            });
        }
    }
}