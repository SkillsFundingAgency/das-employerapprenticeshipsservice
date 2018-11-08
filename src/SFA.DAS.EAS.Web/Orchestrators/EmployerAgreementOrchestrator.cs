using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTeamUser;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;
using System;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.Authorization;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Web.Orchestrators
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


        public async Task<OrchestratorResponse<AddLegalEntityViewModel>> GetAddLegalEntityViewModel(string hashedAccountId, string externalUserId)
        {
            var userRole = await GetUserAccountRole(hashedAccountId, externalUserId);

            return new OrchestratorResponse<AddLegalEntityViewModel>
            {
                Data = new AddLegalEntityViewModel { HashedAccountId = hashedAccountId },
                Status = userRole.UserRole.Equals(Role.Owner) ? HttpStatusCode.OK : HttpStatusCode.Unauthorized
            };

        }



      
    }
}