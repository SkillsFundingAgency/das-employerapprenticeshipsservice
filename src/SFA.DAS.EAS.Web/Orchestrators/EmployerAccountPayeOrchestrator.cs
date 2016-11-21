using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.AddPayeToNewLegalEntity;
using SFA.DAS.EAS.Application.Commands.AddPayeWithExistingLegalEntity;
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
                AccessToken = !string.IsNullOrEmpty(hmrcResponse.Empref) ? response.Data.AccessToken : "",
                RefreshToken = !string.IsNullOrEmpty(hmrcResponse.Empref) ? response.Data.RefreshToken : ""
                }
            };
            
        }

        public async Task<OrchestratorResponse<BeginNewPayeScheme>> CheckUserIsOwner(string hashedId, string email, string redirectUrl)
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

            return new OrchestratorResponse<BeginNewPayeScheme>
            {
                Data = new BeginNewPayeScheme { HashedId = hashedId} ,//TODO
                Status = status,
                FlashMessage = new FlashMessageViewModel
                {
                    RedirectButtonMessage = "Return to PAYE schemes"
                },
                RedirectUrl = redirectUrl
            };
        }

        public async Task<List<LegalEntity>> GetLegalEntities(string hashedId, string userId)
        {
            var response = await Mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedId = hashedId,
                UserId = userId
            });

            return response.Entites.LegalEntityList;
        }


        public async Task AddPayeSchemeToAccount(ConfirmNewPayeScheme model, string userId)
        {
            if (model.LegalEntityId == 0)
            {
                await Mediator.SendAsync(new AddPayeToNewLegalEntityCommand
                {
                    HashedId = model.HashedId,
                    AccessToken = model.AccessToken,
                    RefreshToken = model.RefreshToken,
                    LegalEntityCode = model.LegalEntityCode,
                    Empref = model.PayeScheme,
                    ExternalUserId = userId,
                    LegalEntityDate = model.LegalEntityDateOfIncorporation,
                    LegalEntityAddress = model.LegalEntityRegisteredAddress,
                    LegalEntityName = model.LegalEntityName
                });
            }
            else
            {
                await Mediator.SendAsync(new AddPayeToAccountForExistingLegalEntityCommand
                {
                    HashedId = model.HashedId,
                    ExternalUserId = userId,
                    EmpRef = model.PayeScheme,
                    LegalEntityId = model.LegalEntityId,
                    RefreshToken = model.RefreshToken,
                    AccessToken = model.AccessToken
                });
            }
            
        }

        public virtual async Task<OrchestratorResponse<RemovePayeScheme>> GetRemovePayeSchemeModel(RemovePayeScheme model)
        {
            var response = await
                    Mediator.SendAsync(new GetEmployerAccountHashedQuery()
                    {
                        HashedId = model.HashedId,
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
                    HashedId = model.HashedId,
                    UserId = model.UserId,
                    PayeRef = model.PayeRef,
                    RemoveScheme = model.RemoveScheme
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