using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAccountPayeOrchestrator : HmrcOrchestratorBase
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

        public AddNewPayeScheme GetPayeConfirmModel(long accountId)
        {
            //TODO to be replaced with the call to the HmrcGetEmpref Discovery call
            var empRef = $"{Guid.NewGuid().ToString().Substring(0, 3)}/{Guid.NewGuid().ToString().Substring(0, 7)}";

            return new AddNewPayeScheme
            {
                AccountId = accountId,
                PayeScheme = empRef
            };
        }

        public async Task<List<dynamic>>  GetLegalEntities(long accountId)
        {
            throw new NotImplementedException();
        }
    }
}