using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetProviders;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public sealed class EmployerCommitmentsOrchestrator
    {
        private readonly IMediator _mediator;

        public EmployerCommitmentsOrchestrator(IMediator mediator)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            _mediator = mediator;
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAll(long accountid)
        {
            var data = await _mediator.SendAsync(new GetCommitmentsQuery
            {
                Accountid = accountid
            });

            return new OrchestratorResponse<CommitmentListViewModel>
            {
                Data = new CommitmentListViewModel
                {
                    AccountId = accountid,
                    Commitments = data.Commitments
                }
            };
        }

        public async Task<OrchestratorResponse<ExtendedCreateCommitmentViewModel>> GetNew(long accountId, string externalUserId)
        {
            var providers = await _mediator.SendAsync(new GetProvidersQueryRequest());
            var legalEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                Id = accountId,
                UserId = externalUserId
            });
            //GetAccountLegalEntitiesRequest
            return new OrchestratorResponse<ExtendedCreateCommitmentViewModel>
            {
                Data = new ExtendedCreateCommitmentViewModel
                {
                    Commitment = new CreateCommitmentViewModel
                    {
                        AccountId = accountId
                    },
                    Providers = providers.Providers,
                    LegalEntities = legalEntities.Entites.LegalEntityList
                }
            };
        }

        public async Task Create(CreateCommitmentViewModel commitment, string externalUserId)
        {
            var providers = await _mediator.SendAsync(new GetProvidersQueryRequest());
            var legalEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                Id = commitment.AccountId,
                UserId = externalUserId
            });

            var provider = providers.Providers.SingleOrDefault(x => x.Id == commitment.ProviderId);
            var legalEntity = legalEntities.Entites.LegalEntityList.SingleOrDefault(x => x.Id == commitment.LegalEntityId);

            //await _mediator.SendAsync(new CreateCommitmentCommand
            //{

            //});
        }
    }
}