using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAgreementOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public EmployerAgreementOrchestrator(IMediator mediator, ILogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<EmployerAgreementListViewModel> Get(long accountId, string externalUserId)
        {
            var response = await _mediator.SendAsync(new GetAccountEmployerAgreementsRequest
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });

            return new EmployerAgreementListViewModel
            {
                AccountId = accountId,
                EmployerAgreements = response.EmployerAgreements
            };
        }
    }
}