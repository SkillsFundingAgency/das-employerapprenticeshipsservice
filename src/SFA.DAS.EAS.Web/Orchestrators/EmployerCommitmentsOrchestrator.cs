﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.ApproveApprenticeship;
using SFA.DAS.EAS.Application.Commands.CreateApprenticeship;
using SFA.DAS.EAS.Application.Commands.CreateCommitment;
using SFA.DAS.EAS.Application.Commands.PauseApprenticeship;
using SFA.DAS.EAS.Application.Commands.ResumeApprenticeship;
using SFA.DAS.EAS.Application.Commands.SubmitCommitment;
using SFA.DAS.EAS.Application.Commands.UpdateApprenticeship;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Application.Queries.GetCommitment;
using SFA.DAS.EAS.Application.Queries.GetCommitments;
using SFA.DAS.EAS.Application.Queries.GetProvider;
using SFA.DAS.EAS.Application.Queries.GetProviders;
using SFA.DAS.EAS.Application.Queries.GetStandards;
using SFA.DAS.EAS.Application.Queries.GetTasks;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetFrameworks;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public sealed class EmployerCommitmentsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly ILogger _logger;

        public EmployerCommitmentsOrchestrator(IMediator mediator, IHashingService hashingService, ILogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _hashingService = hashingService;
            _logger = logger;
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAll(string hashId)
        {
            _logger.Debug("Getting all Commitments");

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
                UserId = externalUserId,
                SignedOnly = false //TODO: This should be true when signed agreements is being used
            });

            return new OrchestratorResponse<IList<LegalEntity>>
            {
                Data = legalEntities.Entites.LegalEntityList
            };
        }

        public async Task<OrchestratorResponse<IList<Provider>>> GetProviders(string hashedAccountId, string externalUserId)
        {
            var providers = await GetProviders();

            return new OrchestratorResponse<IList<Provider>>
            {
                Data = providers.Providers
            };
        }

        public async Task<OrchestratorResponse<CreateCommitmentViewModel>> CreateSummary(string hashedAccountId, string legalEntityCode, string providerId, string externalUserId)
        {
            var providers = await GetProvider(int.Parse(providerId));
            var provider = providers.Single(x => x.Ukprn == int.Parse(providerId));

            var legalEntities = await GetActiveLegalEntities(hashedAccountId, externalUserId);
            var legalEntity = legalEntities.Entites.LegalEntityList.Single(x => x.Code.Equals(legalEntityCode, StringComparison.InvariantCultureIgnoreCase));

            return new OrchestratorResponse<CreateCommitmentViewModel>
            {
                Data = new CreateCommitmentViewModel
                {
                    HashedAccountId = hashedAccountId,
                    LegalEntityCode = legalEntityCode,
                    LegalEntityName = legalEntity.Name,
                    ProviderId = provider.Ukprn,
                    ProviderName = provider.ProviderName
                }
            };
        }

        public async Task<string> Create(CreateCommitmentViewModel commitment, string externalUserId)
        {
            var response = await _mediator.SendAsync(new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    Reference = commitment.CohortRef,
                    EmployerAccountId = _hashingService.DecodeValue(commitment.HashedAccountId),
                    LegalEntityId = commitment.LegalEntityCode,
                    LegalEntityName = commitment.LegalEntityName,
                    ProviderId = commitment.ProviderId,
                    ProviderName = commitment.ProviderName,
                    CommitmentStatus = (commitment.SelectedRoute == "employer") ? CommitmentStatus.New : CommitmentStatus.Active
                }
            });

            return _hashingService.HashValue(response.CommitmentId);
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

            var apprenticeship = MapFrom(data.Apprenticeship);

            apprenticeship.HashedAccountId = hashedAccountId;

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes()
            };
        }

        public async Task CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            await _mediator.SendAsync(new CreateApprenticeshipCommand
            {
                AccountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId),
                Apprenticeship = await MapFrom(apprenticeship)
            });
        }

        public async Task UpdateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            await _mediator.SendAsync(new UpdateApprenticeshipCommand
            {
                AccountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId),
                Apprenticeship = await MapFrom(apprenticeship)
            });
        }

        public async Task<ExtendedApprenticeshipViewModel> GetSkeletonApprenticeshipDetails(string hashedAccountId, string hashedCommitmentId)
        {
            var apprenticeship = new ApprenticeshipViewModel
            {
                HashedAccountId = hashedAccountId,
                HashedCommitmentId = hashedCommitmentId,
            };

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes()
            };
        }

        public async Task<string> SubmitCommitment(string hashedAccountId, string hashedCommitmentId, string legalEntityCode, string legalEntityName, string providerId, string providerName, string cohortRef, string message, string saveOrSend)
        {
            var commitmentId = 0L;

            if (string.IsNullOrWhiteSpace(hashedCommitmentId))
            {
                var response = await _mediator.SendAsync(new CreateCommitmentCommand
                {
                    Commitment = new Commitment
                    {
                        Reference = cohortRef,
                        EmployerAccountId = _hashingService.DecodeValue(hashedAccountId),
                        LegalEntityId = legalEntityCode,
                        LegalEntityName = legalEntityName,
                        ProviderId = long.Parse(providerId),
                        ProviderName = providerName
                    }
                });

                commitmentId = response.CommitmentId;
            }
            else
            {
                commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            }

            await _mediator.SendAsync(new SubmitCommitmentCommand
            {
                EmployerAccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = commitmentId,
                Message = message,
                SaveOrSend = saveOrSend
            });

            return _hashingService.HashValue(commitmentId);
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

        public async Task<List<Provider>> GetProvider(int providerId)
        {
            var data = await _mediator.SendAsync(new GetProviderQueryRequest
            {
                ProviderId = providerId
            });

            return data?.ProvidersView?.Providers;
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
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Status = commitment.CommitmentStatus,
                Apprenticeships = commitment.Apprenticeships?.Select(x => MapFrom(x)).ToList() ?? new List<ApprenticeshipViewModel>(0)
            };
        }

        private CommitmentListItemViewModel MapFrom(CommitmentListItem commitment)
        {
            return new CommitmentListItemViewModel
            {
                HashedId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Status = commitment.CommitmentStatus
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
                TrainingId = apprenticeship.TrainingCode,
                TrainingName = apprenticeship.TrainingName,
                Cost = NullableDecimalToString(apprenticeship.Cost),
                StartMonth = apprenticeship.StartDate?.Month,
                StartYear = apprenticeship.StartDate?.Year,
                EndMonth = apprenticeship.EndDate?.Month,
                EndYear = apprenticeship.EndDate?.Year,
                PaymentStatus = apprenticeship.PaymentStatus,
                AgreementStatus = apprenticeship.AgreementStatus
            };
        }

        private static string NullableDecimalToString(decimal? item)
        {
            return (item.HasValue) ? ((int)item).ToString() : "";
        }

        private async Task<Apprenticeship> MapFrom(ApprenticeshipViewModel viewModel)
        {
            var apprenticeship = new Apprenticeship
            {
                CommitmentId = _hashingService.DecodeValue(viewModel.HashedCommitmentId),
                Id = string.IsNullOrWhiteSpace(viewModel.HashedId) ? 0L : _hashingService.DecodeValue(viewModel.HashedId),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                ULN = viewModel.ULN,
                Cost = viewModel.Cost == null ? default(decimal?) : decimal.Parse(viewModel.Cost),
                StartDate = GetDateTime(viewModel.StartMonth, viewModel.StartYear),
                EndDate = GetDateTime(viewModel.EndMonth, viewModel.EndYear)
            };

            if (!string.IsNullOrWhiteSpace(viewModel.TrainingId))
            {
                var training = await GetTrainingProgramme(viewModel.TrainingId);
                apprenticeship.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                apprenticeship.TrainingCode = viewModel.TrainingId;
                apprenticeship.TrainingName = training.Title;
            }

            return apprenticeship;
        }

        private async Task<ITrainingProgramme> GetTrainingProgramme(string trainingCode)
        {
            var id = int.Parse(trainingCode);

            return (await GetTrainingProgrammes()).Where(x => x.Id == id).Single();
        }

        private DateTime? GetDateTime(int? month, int? year)
        {
            if (month.HasValue && year.HasValue)
                return new DateTime(year.Value, month.Value, 1);

            return null;
        }

        private async Task<List<ITrainingProgramme>> GetTrainingProgrammes()
        {
            var standardsTask = _mediator.SendAsync(new GetStandardsQueryRequest());
            var frameworksTask = _mediator.SendAsync(new GetFrameworksQueryRequest());

            await Task.WhenAll(standardsTask, frameworksTask);

            return standardsTask.Result.Standards.Union(frameworksTask.Result.Frameworks.Cast<ITrainingProgramme>()).ToList();
        }
    }
}