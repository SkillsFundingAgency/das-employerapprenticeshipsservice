using System;
using System.Collections.Generic;
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
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetProviders;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetTasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public sealed class EmployerCommitmentsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;

        public EmployerCommitmentsOrchestrator(IMediator mediator, IHashingService hashingService)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));

            _mediator = mediator;
            _hashingService = hashingService;
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAll(string hashId)
        {
            var data = await _mediator.SendAsync(new GetCommitmentsQuery
            {
                AccountHashId = hashId
            });

            var tasks = await _mediator.SendAsync(new GetTasksQueryRequest
            {
                AccountHashId = hashId
            });

            return new OrchestratorResponse<CommitmentListViewModel>
            {
                Data = new CommitmentListViewModel
                {
                    AccountHashId = hashId,
                    Commitments = data.Commitments.Select(x => MapFrom(x)).ToList(),
                    NumberOfTasks = tasks.Tasks.Count
                }
            };
        }

        public async Task<OrchestratorResponse<IList<LegalEntity>>> GetLegalEntities(string hashedAccountId, string externalUserId)
        {
            var legalEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedId = hashedAccountId,
                UserId = externalUserId
            });

            return new OrchestratorResponse<IList<LegalEntity>>
            {
                Data = legalEntities.Entites.LegalEntityList
            };
        }

        public async Task<OrchestratorResponse<ExtendedCreateCommitmentViewModel>> GetProviders(string hashedAccountId, string externalUserId)
            return new OrchestratorResponse<ExtendedCreateCommitmentViewModel>
                Data = providers.Providers
            };
        }

        public async Task<OrchestratorResponse<CreateCommitmentViewModel>> CreateSummary(string hashedAccountId, string legalEntityCode, string providerId, string externalUserId)
        {
            var providers = await GetProviders();
            var legalEntities = await GetActiveLegalEntities(hashedAccountId, externalUserId);

            var provider = providers.Providers.Single(x => x.Id == long.Parse(providerId));
            var legalEntity = legalEntities.Entites.LegalEntityList.Single(x => x.Code.Equals(legalEntityCode, StringComparison.InvariantCultureIgnoreCase));

            return new OrchestratorResponse<CreateCommitmentViewModel>
            {
                Data = new CreateCommitmentViewModel
                {
                    HashedAccountId = hashedAccountId,
                    LegalEntityCode = legalEntityCode,
                    LegalEntityName = legalEntity.Name,
                    ProviderId = provider.Id,
                    ProviderName = provider.Name
                }
            };
        }

        public async Task Create(CreateCommitmentViewModel commitment, string externalUserId)
        {
            var providers = await GetProviders();
            var legalEntities = await GetActiveLegalEntities(commitment.HashedAccountId, externalUserId);

            var provider = providers.Providers.Single(x => x.Id == commitment.ProviderId);
            var legalEntity = legalEntities.Entites.LegalEntityList.Single(x => x.Code.Equals(commitment.LegalEntityCode, StringComparison.InvariantCultureIgnoreCase));

            await _mediator.SendAsync(new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    Name = commitment.Name,
                    EmployerAccountId = _hashingService.DecodeValue(commitment.HashedAccountId),
                    LegalEntityCode = commitment.LegalEntityCode,
                    LegalEntityName = legalEntity.Name,
                    ProviderId = commitment.ProviderId,
                    ProviderName = provider.Name
                }
            });
        }

        public async Task ApproveApprenticeship(ApproveApprenticeshipModel model)
        {
            await _mediator.SendAsync(new ApproveApprenticeshipCommand
            {
                EmployerAccountId = _hashingService.DecodeValue(model.HashedAccountId),
                CommitmentId = _hashingService.DecodeValue(model.HashedCommitmentId),
                ApprenticeshipId = _hashingService.DecodeValue(model.HashedApprenticeshipId)
            });
        }

        public async Task<CommitmentViewModel> Get(string hashedAccountId, string hashedCommitmentId)
        {
            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                AccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId)
            });

            return MapFrom(data.Commitment);
        }

        public async Task<ExtendedApprenticeshipViewModel> GetApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                AccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId),
                ApprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId)
            });

            var standards = await _mediator.SendAsync(new GetStandardsQueryRequest());

            var apprenticeship = MapFrom(data.Apprenticeship);

            apprenticeship.HashedAccountId = hashedAccountId;

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
                AccountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId),
                Apprenticeship = MapFrom(apprenticeship)
            });
        }

        public async Task<ExtendedApprenticeshipViewModel> GetSkeletonApprenticeshipDetails(string hashedAccountId, string hashedCommitmentId)
        {
            var standards = await _mediator.SendAsync(new GetStandardsQueryRequest());

            var apprenticeship = new ApprenticeshipViewModel
            {
                HashedAccountId = hashedAccountId,
                HashedCommitmentId = hashedCommitmentId,
            };

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                Standards = standards.Standards
            };
        }

        public async Task SubmitCommitment(string hashedAccountId, string hashedCommitmentId, string message)
        {
            await _mediator.SendAsync(new SubmitCommitmentCommand
            {
                EmployerAccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId),
                Message = message
            });
        }
        
	    public async Task PauseApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            await _mediator.SendAsync(new PauseApprenticeshipCommand
            {
                EmployerAccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId),
                ApprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId)
            });
        }

        public async Task ResumeApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            await _mediator.SendAsync(new ResumeApprenticeshipCommand
            {
                EmployerAccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId),
                ApprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId)
            });
        }

        private async Task<GetProvidersQueryResponse> GetProviders()
        {
            return await _mediator.SendAsync(new GetProvidersQueryRequest());
        }

        private async Task<GetAccountLegalEntitiesResponse> GetActiveLegalEntities(string hashedAccountId, string externalUserId)
        {
            return await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedId = hashedAccountId,
                UserId = externalUserId
            });
        }

        private CommitmentViewModel MapFrom(Commitment commitment)
        {
            return new CommitmentViewModel
            {
                HashedId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Name,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Status = commitment.Status,
                Apprenticeships = commitment.Apprenticeships?.Select(x => MapFrom(x)).ToList() ?? new List<ApprenticeshipViewModel>(0)
            };
        }

        private CommitmentListItemViewModel MapFrom(CommitmentListItem commitment)
        {
            return new CommitmentListItemViewModel
            {
                HashedId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Name,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Status = commitment.Status
            };
        }

        private ApprenticeshipViewModel MapFrom(Apprenticeship apprenticeship)
        {
            return new ApprenticeshipViewModel
            {
                HashedId = _hashingService.HashValue(apprenticeship.Id),
                HashedCommitmentId = _hashingService.HashValue(apprenticeship.CommitmentId),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                ULN = apprenticeship.ULN,
                TrainingType = apprenticeship.TrainingType,
                TrainingCode = apprenticeship.TrainingCode,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost.ToString(),
                StartMonth = apprenticeship.StartDate?.Month,
                StartYear = apprenticeship.StartDate?.Year,
                EndMonth = apprenticeship.EndDate?.Month,
                EndYear = apprenticeship.EndDate?.Year,
                Status = apprenticeship.Status,
                AgreementStatus = apprenticeship.AgreementStatus.ToString()
            };
        }

        private Apprenticeship MapFrom(ApprenticeshipViewModel viewModel)
        {
            return new Apprenticeship
            {
                CommitmentId = _hashingService.DecodeValue(viewModel.HashedCommitmentId),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                ULN = viewModel.ULN,
                TrainingType = viewModel.TrainingType,
                TrainingCode = viewModel.TrainingCode,
                TrainingName = viewModel.TrainingName,
                Cost = viewModel.Cost == null ? default(decimal?) : decimal.Parse(viewModel.Cost),
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

        public async Task<List<Provider>> FindProviders(int providerId)
        {
            var data = await _mediator.SendAsync(new GetProviderQueryRequest
            {
                ProviderId = providerId
            });

            return data.ProvidersView.Providers;
        }
    }
}