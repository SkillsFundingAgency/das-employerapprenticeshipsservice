using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ApproveApprenticeship;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateCommitment;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetProviders;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTasks;
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

            var tasks = await _mediator.SendAsync(new GetTasksQueryRequest
            {
                AccountId = accountid
            });

            return new OrchestratorResponse<CommitmentListViewModel>
            {
                Data = new CommitmentListViewModel
                {
                    AccountId = accountid,
                    Commitments = data.Commitments,
                    NumberOfTasks = tasks.Tasks.Count
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

            await _mediator.SendAsync(new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    Name = commitment.Name,
                    EmployerAccountId = commitment.AccountId,
                    LegalEntityId = commitment.LegalEntityId,
                    LegalEntityName = legalEntity.Name,
                    ProviderId = commitment.ProviderId,
                    ProviderName = provider?.Name
                }
            });
        }

        public async Task ApproveApprenticeship(long accountId, long commitmentId, long apprenticeshipId)
        {
            await _mediator.SendAsync(new ApproveApprenticeshipCommand { EmployerAccountId = accountId, CommitmentId = commitmentId, ApprenticeshipId = apprenticeshipId });

            // TODO: LWA - Extend create task command.
            //await _mediator.SendAsync(new CreateTaskCommand { ProviderId = providerId });
        }

        public async Task<CommitmentViewModel> Get(long accountId, long commitmentId)
        {
            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                AccountId = accountId,
                CommitmentId = commitmentId
            });

            return new CommitmentViewModel
            {
                Commitment = data.Commitment
            };
        }

        public async Task<object> GetApprenticeship(long accountId, long commitmentId, long apprenticeshipId)
        {
            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                AccountId = accountId,
                CommitmentId = commitmentId,
                ApprenticeshipId = apprenticeshipId
            });

            var standards = await _mediator.SendAsync(new GetStandardsQueryRequest());

            var apprenticeship = MapFrom(data.Apprenticeship);

            apprenticeship.AccountId = accountId;

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                Standards = standards.Standards
            };
        }

        public async Task SubmitCommitment(long accountId, long commitmentId)
        {
            await _mediator.SendAsync(new SubmitCommitmentCommand { EmployerAccountId = accountId, CommitmentId = commitmentId });
        }

        private ApprenticeshipViewModel MapFrom(Apprenticeship apprenticeship)
        {
            return new ApprenticeshipViewModel
            {
                Id = apprenticeship.Id,
                CommitmentId = apprenticeship.CommitmentId,
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                ULN = apprenticeship.ULN,
                TrainingId = apprenticeship.TrainingId,
                Cost = apprenticeship.Cost,
                StartMonth = apprenticeship.StartDate?.Month,
                StartYear = apprenticeship.StartDate?.Year,
                EndMonth = apprenticeship.EndDate?.Month,
                EndYear = apprenticeship.EndDate?.Year,
                Status = apprenticeship.Status.ToString(),
                AgreementStatus = apprenticeship.AgreementStatus.ToString()
            };
        }
    }
}