using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class EmployerAgreementOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly IReferenceDataService _referenceDataService;

        protected EmployerAgreementOrchestrator()
        {
        }

        public EmployerAgreementOrchestrator(
            IMediator mediator,
            ILog logger,
            IMapper mapper,
            EmployerApprenticeshipsServiceConfiguration configuration,
            IReferenceDataService referenceDataService) : base(mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (referenceDataService == null)
                throw new ArgumentNullException(nameof(referenceDataService));

            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _referenceDataService = referenceDataService;
        }

        public virtual async Task<OrchestratorResponse<EmployerAgreementListViewModel>> Get(string hashedId,
            string externalUserId)
        {
            try
            {
                var response = await _mediator.SendAsync(new GetAccountEmployerAgreementsRequest
                {
                    HashedAccountId = hashedId,
                    ExternalUserId = externalUserId
                });

                return new OrchestratorResponse<EmployerAgreementListViewModel>
                {
                    Data = new EmployerAgreementListViewModel
                    {
                        HashedAccountId = hashedId,
                        EmployerAgreementsData = response
                    }
                };
            }
            catch (Exception)
            {
                return new OrchestratorResponse<EmployerAgreementListViewModel>
                {
                    Status = HttpStatusCode.Unauthorized
                };
            }
        }
    }
}