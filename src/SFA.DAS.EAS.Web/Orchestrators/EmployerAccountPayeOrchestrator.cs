using System;
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
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class EmployerAccountPayeOrchestrator : EmployerVerificationOrchestratorBase
    {
        protected EmployerAccountPayeOrchestrator()
        {
            
        }

        public EmployerAccountPayeOrchestrator(IMediator mediator, ILogger logger, ICookieStorageService<EmployerAccountData> cookieService, EmployerApprenticeshipsServiceConfiguration configuration) : base(mediator, logger, cookieService, configuration)
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

        public async Task<OrchestratorResponse<AddNewPayeSchemeViewModel>> GetPayeConfirmModel(string hashedId, string code, string redirectUrl, NameValueCollection nameValueCollection)
        { 
            var response = await GetGatewayTokenResponse(code, redirectUrl, nameValueCollection);
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
            

            return new OrchestratorResponse < AddNewPayeSchemeViewModel > { 
               Data = new AddNewPayeSchemeViewModel
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
        

        public virtual async Task<OrchestratorResponse<AddNewPayeSchemeViewModel>> AddPayeSchemeToAccount(AddNewPayeSchemeViewModel model, string userId)
        {
            var response = new OrchestratorResponse<AddNewPayeSchemeViewModel> { Data = model };

            try
            {
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
            var accountResponse = await
                    Mediator.SendAsync(new GetEmployerAccountHashedQuery
                    {
                        HashedAccountId = model.HashedAccountId,
                        UserId = model.UserId
                    });

            var payeResponse = await
                Mediator.SendAsync(new GetPayeSchemeByRefQuery
                {
                    HashedAccountId = model.HashedAccountId,
                    Ref = model.PayeRef
                });

            model.AccountName = accountResponse.Account.Name;
            model.PayeSchemeName = payeResponse.PayeScheme.Name;

            return new OrchestratorResponse<RemovePayeSchemeViewModel> {Data = model};
        }

        public virtual async Task<OrchestratorResponse<RemovePayeSchemeViewModel>> RemoveSchemeFromAccount(RemovePayeSchemeViewModel model)
        {
            var response = new OrchestratorResponse<RemovePayeSchemeViewModel> {Data = model};
            try
            {
                var result = await Mediator.SendAsync(new GetPayeSchemeByRefQuery
                {
                    HashedAccountId = model.HashedAccountId,
                    Ref = model.PayeRef,
                });

                model.PayeSchemeName = result.PayeScheme.Name;

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

        public async Task<OrchestratorResponse<PayeSchemeDetailViewModel>>  GetPayeDetails(string empRef, string hashedAccountId, string userId)
        {
            var response = new OrchestratorResponse<PayeSchemeDetailViewModel>();
            try
            {
                var englishFractionResult = await Mediator.SendAsync(new GetEmployerEnglishFractionQuery
                {
                    HashedAccountId = hashedAccountId,
                    EmpRef = empRef,
                    UserId = userId
                });

                var payeSchemeResult = await Mediator.SendAsync(new GetPayeSchemeByRefQuery
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
            catch (UnauthorizedAccessException )
            {
                response.Status = HttpStatusCode.Unauthorized;
            }
            
            return response;
        }
    }
}