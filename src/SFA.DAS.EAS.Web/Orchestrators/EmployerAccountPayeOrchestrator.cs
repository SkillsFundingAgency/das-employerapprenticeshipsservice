using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.AddPayeToAccount;
using SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetMember;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Entities.Account;
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

        public async Task<OrchestratorResponse<EmployerAccountPayeListViewModel>> Get(string hashedId, string externalUserId)
        {
            var response = await Mediator.SendAsync(new GetAccountPayeSchemesRequest
            {
                HashedId = hashedId,
                ExternalUserId = externalUserId
            });
            
            return new OrchestratorResponse<EmployerAccountPayeListViewModel>
            {
                Data = new EmployerAccountPayeListViewModel
                {
                    HashedId = hashedId,
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
                        HashedId = hashedId
                    },
                    Status = response.Status,
                    
                    FlashMessage = response.FlashMessage,
                    
                };
            }

            var hmrcResponse = await GetHmrcEmployerInformation(response.Data.AccessToken, string.Empty);
            

            return new OrchestratorResponse < AddNewPayeScheme > { 
               Data = new AddNewPayeScheme
            {
                 
                HashedId = hashedId,
                PayeScheme = hmrcResponse.Empref,
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
                HashedId = hashedId,
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
        

        public async Task AddPayeSchemeToAccount(AddNewPayeScheme model, string userId)
        {
            
            await Mediator.SendAsync(new AddPayeToAccountCommand
            {
                HashedId = model.HashedId,
                AccessToken = model.AccessToken,
                RefreshToken = model.RefreshToken,
                Empref = model.PayeScheme,
                ExternalUserId = userId,
            });
            
            
        }

        public virtual async Task<OrchestratorResponse<RemovePayeScheme>> GetRemovePayeSchemeModel(RemovePayeScheme model)
        {
            var response = await
                    Mediator.SendAsync(new GetEmployerAccountHashedQuery()
                    {
                        HashedId = model.AccountHashedId,
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
                    HashedId = model.AccountHashedId,
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
    }
}