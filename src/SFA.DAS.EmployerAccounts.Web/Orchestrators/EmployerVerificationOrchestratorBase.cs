﻿using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetGatewayInformation;
using SFA.DAS.EmployerAccounts.Queries.GetGatewayToken;
using SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Hmrc.Models;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public abstract class EmployerVerificationOrchestratorBase
    {
        protected readonly IMediator Mediator;
        protected readonly ICookieStorageService<EmployerAccountData> CookieService;
        protected readonly EmployerAccountsConfiguration Configuration;

        //Needed for tests
        protected EmployerVerificationOrchestratorBase()
        {
        }

        protected EmployerVerificationOrchestratorBase(IMediator mediator, ICookieStorageService<EmployerAccountData> cookieService, EmployerAccountsConfiguration configuration)
        {
            Mediator = mediator;
            CookieService = cookieService;
            Configuration = configuration;
        }


        public virtual async Task<GetUserAccountRoleResponse> GetUserAccountRole(string hashedAccountId, string externalUserId)
        {
            return await Mediator.SendAsync(new GetUserAccountRoleQuery
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId
            });
        }

        public virtual async Task<string> GetGatewayUrl(string redirectUrl)
        {
            var response = await Mediator.SendAsync(new GetGatewayInformationQuery
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
                if (queryCollection["error_Code"] == "USER_DENIED_AUTHORIZATION")
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
    }
}