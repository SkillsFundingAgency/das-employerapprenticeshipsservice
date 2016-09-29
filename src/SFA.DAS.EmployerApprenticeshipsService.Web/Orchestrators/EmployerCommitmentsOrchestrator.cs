using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ApproveApprenticeship;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateApprenticeship;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateCommitment;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.PauseApprenticeship;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ResumeApprenticeship;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetApprenticeship;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetProviders;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTasks;
using SFA.DAS.EmployerApprenticeshipsService.Web.Extensions;
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

        public async Task<OrchestratorResponse<ExtendedCreateCommitmentViewModel>> GetLegalEntities(long accountId, string externalUserId)
        {
            var legalEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedId = "",
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
                    LegalEntities = legalEntities.Entites.LegalEntityList
                }
            };
        }

        public async Task<OrchestratorResponse<ExtendedCreateCommitmentViewModel>> GetProviders(long accountId, string externalUserId)
        {
            var providers = await GetProviders();

            return new OrchestratorResponse<ExtendedCreateCommitmentViewModel>
            {
                Data = new ExtendedCreateCommitmentViewModel
                {
                    Commitment = new CreateCommitmentViewModel
                    {
                Id = commitment.AccountId,
                    },
                    Providers = providers.Providers
                }
            };
        }

        public async Task<OrchestratorResponse<CreateCommitmentViewModel>> CreateSummary(CreateCommitmentModel commitment, string externalUserId)
        {
            var providers = await GetProviders();
            var legalEntities = await GetActiveLegalEntities(commitment.AccountId, externalUserId);

            var provider = providers.Providers.Single(x => x.Id == commitment.ProviderId);
            var legalEntity = legalEntities.Entites.LegalEntityList.Single(x => x.Id == commitment.LegalEntityId);

            return new OrchestratorResponse<CreateCommitmentViewModel>
            {
                Data = new CreateCommitmentViewModel
                {
                    AccountId = commitment.AccountId,
                    LegalEntityId = commitment.LegalEntityId,
                    LegalEntityName = legalEntity.Name,
                    ProviderId = commitment.ProviderId,
                    ProviderName = provider.Name
                }
            };
        }

        public async Task Create(CreateCommitmentViewModel commitment, string externalUserId)
        {
            await _mediator.SendAsync(new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    Name = commitment.Name,
                    EmployerAccountId = commitment.AccountId,
                    LegalEntityId = commitment.LegalEntityId,
                    LegalEntityName = commitment.LegalEntityName,
                    ProviderId = commitment.ProviderId,
                    ProviderName = commitment.ProviderName
                }
            });
        }

        public async Task ApproveApprenticeship(ApproveApprenticeshipModel model)
        {
            await _mediator.SendAsync(new ApproveApprenticeshipCommand { EmployerAccountId = model.AccountId, CommitmentId = model.CommitmentId, ApprenticeshipId = model.ApprenticeshipId, Message = model.Message });
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

        public async Task<ExtendedApprenticeshipViewModel> GetApprenticeship(long accountId, long commitmentId, long apprenticeshipId)
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

        public async Task CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            await _mediator.SendAsync(new CreateApprenticeshipCommand
            {
                AccountId = apprenticeship.AccountId,
                Apprenticeship = MapFrom(apprenticeship)
            });
        }

        public async Task<ExtendedApprenticeshipViewModel> GetSkeletonApprenticeshipDetails(long accountId, long commitmentId)
        {
            var standards = await _mediator.SendAsync(new GetStandardsQueryRequest());

            var apprenticeship = new ApprenticeshipViewModel
            {
                AccountId = accountId,
                CommitmentId = commitmentId,
            };

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                Standards = standards.Standards
            };
        }

        public async Task SubmitCommitment(long accountId, long commitmentId, string message)
        {
            await _mediator.SendAsync(new SubmitCommitmentCommand
            {
                EmployerAccountId = accountId,
                CommitmentId = commitmentId,
                Message = message
            });
        }
        
	    public async Task PauseApprenticeship(long accountId, long commitmentId, long apprenticeshipId)
        {
            await _mediator.SendAsync(new PauseApprenticeshipCommand
            {
                EmployerAccountId = accountId,
                CommitmentId = commitmentId,
                ApprenticeshipId = apprenticeshipId
            });
        }

        public async Task ResumeApprenticeship(long accountId, long commitmentId, long apprenticeshipId)
        {
            await _mediator.SendAsync(new ResumeApprenticeshipCommand
            {
                EmployerAccountId = accountId,
                CommitmentId = commitmentId,
                ApprenticeshipId = apprenticeshipId
            });
        }

        private async Task<GetProvidersQueryResponse> GetProviders()
        {
            return await _mediator.SendAsync(new GetProvidersQueryRequest());
        }

        private async Task<GetAccountLegalEntitiesResponse> GetActiveLegalEntities(long accountId, string externalUserId)
        {
            return await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                Id = accountId,
                UserId = externalUserId
            });
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
                StartMonth = apprenticeship.StartDate.Value.Month,
                StartYear = apprenticeship.StartDate?.Year,
                EndMonth = apprenticeship.EndDate.Value.Month,
                EndYear = apprenticeship.EndDate?.Year,
                Status = apprenticeship.Status.GetDescription(),
                AgreementStatus = apprenticeship.AgreementStatus.ToString()
            };
        }

        private Apprenticeship MapFrom(ApprenticeshipViewModel viewModel)
        {
            return new Apprenticeship
            {
                Id = viewModel.Id,
                CommitmentId = viewModel.CommitmentId,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                ULN = viewModel.ULN,
                TrainingId = viewModel.TrainingId,
                Cost = viewModel.Cost,
                StartDate = GetDateTime(viewModel.StartMonth, viewModel.StartYear),
                EndDate = GetDateTime(viewModel.EndMonth, viewModel.EndYear)
            };
        }

        private DateTime? GetDateTime(int? month, int? year)
        {
            if (month.HasValue && year.HasValue)
                return new DateTime(year.Value, month.Value, 1);

            return null;
        }
    }
}