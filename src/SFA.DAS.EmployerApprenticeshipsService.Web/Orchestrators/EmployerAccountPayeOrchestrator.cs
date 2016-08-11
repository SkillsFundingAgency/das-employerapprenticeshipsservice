using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeWithExistingLegalEntity;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.HmrcLevy;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAccountPayeOrchestrator : EmployerVerificationOrchestratorBase
    {

        public EmployerAccountPayeOrchestrator(IMediator mediator, ILogger logger, ICookieService cookieService) : base(mediator, logger, cookieService)
        {
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

            //TODO to be replaced with the call to the HmrcGetEmpref Discovery call
            var empRef = $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}";
            

            return new AddNewPayeScheme
            {
                AccountId = accountId,
                PayeScheme = empRef,
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken
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