using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeWithExistingLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAccountPayeOrchestrator : EmployerVerificationOrchestratorBase
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public EmployerAccountPayeOrchestrator(IMediator mediator, ILogger logger, ICookieService cookieService, EmployerApprenticeshipsServiceConfiguration configuration) : base(mediator, logger, cookieService)
        {
            _configuration = configuration;
        }

        public async Task<List<PayeView>> Get(long accountId, string externalUserId)
        {
            var response = await Mediator.SendAsync(new GetAccountPayeSchemesRequest
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });

            return response.PayeSchemes;
        }

        public async Task<AddNewPayeScheme> GetPayeConfirmModel(long accountId, string code, string redirectUrl)
        {
            var response = await GetGatewayTokenResponse(code, redirectUrl);

            string empRef;
            if (_configuration.Hmrc.IgnoreDuplicates)
            {
                empRef = $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}";
            }
            else
            {
                var hmrcResponse = await GetHmrcEmployerInformation(response.AccessToken);
                empRef = hmrcResponse.Empref;

                var schemeCheck = await Mediator.SendAsync(new GetPayeSchemeInUseQuery {Empref= empRef});

                if (schemeCheck != null)
                {
                    empRef = "";
                }
            }

            return new AddNewPayeScheme
            {
                
                AccountId = accountId,
                PayeScheme = empRef,
                AccessToken = !string.IsNullOrEmpty(empRef) ? response.AccessToken : "",
                RefreshToken = !string.IsNullOrEmpty(empRef) ? response.RefreshToken : ""
                
            };
            
        }

        public async Task<List<LegalEntity>> GetLegalEntities(long accountId, string userId)
        {
            var response = await Mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                Id = accountId,
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
                    AccountId = model.AccountId,
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
                    AccountId = model.AccountId,
                    ExternalUserId = userId,
                    EmpRef = model.PayeScheme,
                    LegalEntityId = model.LegalEntityId,
                    RefreshToken = model.RefreshToken,
                    AccessToken = model.AccessToken
                });
            }
            
        }
    }
}