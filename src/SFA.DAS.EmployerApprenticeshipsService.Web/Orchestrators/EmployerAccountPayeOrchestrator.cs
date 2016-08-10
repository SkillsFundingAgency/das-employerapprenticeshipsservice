using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerApprenticeshipsService.Domain;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class EmployerAccountPayeOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly ICookieService _cookieService;

        public EmployerAccountPayeOrchestrator(IMediator mediator, ILogger logger, ICookieService cookieService)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (cookieService == null)
                throw new ArgumentNullException(nameof(cookieService));
            _mediator = mediator;
            _logger = logger;
            _cookieService = cookieService;
        }

        public async Task<List<PayeView>> Get(long accountId, string externalUserId)
        {
            var response = await _mediator.SendAsync(new GetAccountPayeSchemesRequest
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });

            return response.PayeSchemes;
        }
    }
}