﻿using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.AddPayeToAccount;
using SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount;
using SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EAS.Application.Queries.GetMember;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAccountPayeOrchestrator : EmployerVerificationOrchestratorBase
    {
        protected EmployerAccountPayeOrchestrator()
        {
            
        }

        public EmployerAccountPayeOrchestrator(IMediator mediator, ILogger logger, ICookieService cookieService, EmployerApprenticeshipsServiceConfiguration configuration) : base(mediator, logger, cookieService, configuration)
        {
        }

        public async Task<OrchestratorResponse<EmployerAccountPayeListViewModel>> Get(string hashedAccountId, string externalUserId)
        {
            var response = await Mediator.SendAsync(new GetAccountPayeSchemesQuery
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId
            });
            
            return new OrchestratorResponse<EmployerAccountPayeListViewModel>
            {
                Data = new EmployerAccountPayeListViewModel
                {
                    HashedId = hashedAccountId,
                    PayeSchemes = response.PayeSchemes
                }
            };
        }

        public async Task<OrchestratorResponse<AddNewPayeScheme>> GetPayeConfirmModel(string hashedId, string code, string redirectUrl, NameValueCollection nameValueCollection)
        { 
            var response = await GetGatewayTokenResponse(code, redirectUrl, nameValueCollection);
            if (response.Status != HttpStatusCode.OK)
            {
                response.FlashMessage.ErrorMessages.Clear();
                response.FlashMessage.ErrorMessages.Add("addNewPaye", "Add new scheme");
                response.FlashMessage.Headline = "PAYE scheme not added";
                response.FlashMessage.Message = "You need to grant authority to HMRC to add a PAYE scheme.";

                return new OrchestratorResponse<AddNewPayeScheme>
                {
                    Data = new AddNewPayeScheme
                    {
                        HashedAccountId = hashedId
                    },
                    Status = response.Status,
                    
                    FlashMessage = response.FlashMessage,
                    
                };
            }

            var hmrcResponse = await GetHmrcEmployerInformation(response.Data.AccessToken, string.Empty);
            

            return new OrchestratorResponse < AddNewPayeScheme > { 
               Data = new AddNewPayeScheme
            {
                 
                HashedAccountId = hashedId,
                PayeScheme = hmrcResponse.Empref,
                PayeName = hmrcResponse?.EmployerLevyInformation?.Employer?.Name?.EmprefAssociatedName ?? "",
                EmprefNotFound = hmrcResponse.EmprefNotFound,
                AccessToken = !string.IsNullOrEmpty(hmrcResponse.Empref) ? response.Data.AccessToken : "",
                RefreshToken = !string.IsNullOrEmpty(hmrcResponse.Empref) ? response.Data.RefreshToken : ""
                }
            };
            
        }

        public async Task<OrchestratorResponse<GatewayInformViewModel>> CheckUserIsOwner(string hashedId, string email, string redirectUrl, string breadcrumbUrl, string confirmUrl)
        {
            HttpStatusCode status = HttpStatusCode.OK;

            
            var response = await Mediator.SendAsync(new GetMemberRequest
            {
                HashedAccountId = hashedId,
                Email = email
            });

            if (response != null)
            {
                if (response.TeamMember.Role != Role.Owner)
                {
                    status = HttpStatusCode.Unauthorized;
                }
            }

            return new OrchestratorResponse<GatewayInformViewModel>
            {
                Data = new GatewayInformViewModel
                {
                    BreadcrumbUrl = breadcrumbUrl,
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
        

        public virtual async Task AddPayeSchemeToAccount(AddNewPayeScheme model, string userId)
        {
            //TODO change to return the OrchestratorResposne as this can have a unauthorized resposne
            await Mediator.SendAsync(new AddPayeToAccountCommand
            {
                HashedAccountId = model.HashedAccountId,
                AccessToken = model.AccessToken,
                RefreshToken = model.RefreshToken,
                Empref = model.PayeScheme,
                ExternalUserId = userId,
                EmprefName = model.PayeName
            });
        }

        public virtual async Task<OrchestratorResponse<RemovePayeScheme>> GetRemovePayeSchemeModel(RemovePayeScheme model)
        {
            var response = await
                    Mediator.SendAsync(new GetEmployerAccountHashedQuery
                    {
                        HashedAccountId = model.HashedAccountId,
                        UserId = model.UserId
                    });

            model.AccountName = response.Account.Name;

            return new OrchestratorResponse<RemovePayeScheme> {Data = model};
        }

        public virtual async Task<OrchestratorResponse<RemovePayeScheme>> RemoveSchemeFromAccount(RemovePayeScheme model)
        {
            var response = new OrchestratorResponse<RemovePayeScheme> {Data = model};
            try
            {
                await Mediator.SendAsync(new RemovePayeFromAccountCommand
                {
                    HashedAccountId = model.HashedAccountId,
                    UserId = model.UserId,
                    PayeRef = model.PayeRef,
                    RemoveScheme = model.RemoveScheme == 2
                });
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

        public async Task<OrchestratorResponse<PayeSchemeDetail>>  GetPayeDetails(string empRef, string hashedAccountId, string userId)
        {
            var response = new OrchestratorResponse<PayeSchemeDetail>();
            try
            {
                var result = await Mediator.SendAsync(new GetEmployerEnglishFractionQuery
                {
                    HashedAccountId = hashedAccountId,
                    EmpRef = empRef,
                    UserId = userId
                });
                response.Data = new PayeSchemeDetail {Fractions = result.Fractions, EmpRef = result.EmpRef, EmpRefAdded = result.EmpRefAddedDate};
                return response;
            }
            catch (UnauthorizedAccessException )
            {
                response.Status = HttpStatusCode.Unauthorized;
            }
            
            return response;
        }
    }
}