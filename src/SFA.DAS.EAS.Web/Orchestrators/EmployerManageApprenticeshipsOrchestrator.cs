using System;
using System.Linq;
using System.Threading.Tasks;

using MediatR;
using NLog;

using SFA.DAS.EAS.Application.Queries.GetAllApprenticeships;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public sealed class EmployerManageApprenticeshipsOrchestrator : CommitmentsBaseOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly ILogger _logger;

        public EmployerManageApprenticeshipsOrchestrator(
            IMediator mediator, 
            IHashingService hashingService,
            IApprenticeshipMapper apprenticeshipMapper,
            ILogger logger) : base(mediator, hashingService, logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (apprenticeshipMapper == null)
                throw new ArgumentNullException(nameof(apprenticeshipMapper));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _hashingService = hashingService;
            _apprenticeshipMapper = apprenticeshipMapper;
            _logger = logger;
        }

        public async Task<OrchestratorResponse<ManageApprenticeshipsViewModel>> GetApprenticeships(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting On-programme apprenticeships for empployer: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                    var data = await _mediator.SendAsync(new GetAllApprenticeshipsRequest { AccountId = accountId });

                    var apprenticeships = data.Apprenticeships
                        .OrderBy(m => m.ApprenticeshipName)
                        .Select(_apprenticeshipMapper.MapFrom)
                        .ToList();

                    var model = new ManageApprenticeshipsViewModel
                                    {
                                        HashedAccountId = hashedAccountId,
                                        Apprenticeships = apprenticeships
                                    };

                return new OrchestratorResponse<ManageApprenticeshipsViewModel>
                           {
                               Data = model
                           };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ApprenticeshipDetailsViewModel>> GetApprenticeship(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Getting On-programme apprenticeships Provider: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
                {
                    var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });
                    return new OrchestratorResponse<ApprenticeshipDetailsViewModel>
                               {
                                   Data = _apprenticeshipMapper.MapFrom(data.Apprenticeship)
                               };
                }, hashedAccountId, externalUserId);
        }
    }
}

