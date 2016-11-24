using System;
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
    using System.Globalization;

    public sealed class EmployerCommitmentsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly ILogger _logger;
        private readonly ICommitmentStatusCalculator _statusCalculator;

        public EmployerCommitmentsOrchestrator(IMediator mediator, IHashingService hashingService, ICommitmentStatusCalculator statusCalculator, ILogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (statusCalculator == null)
                throw new ArgumentNullException(nameof(statusCalculator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _hashingService = hashingService;
            _statusCalculator = statusCalculator;
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

        public async Task<string> CreateEmployerAssignedCommitment(CreateCommitmentViewModel commitment)
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
                    CommitmentStatus = CommitmentStatus.New,
                    EditStatus = EditStatus.EmployerOnly
                }
            });

            return _hashingService.HashValue(response.CommitmentId);
        }

        public async Task<string> CreateProviderAssignedCommitment(SubmitCommitmentModel model)
        {
            var response = await _mediator.SendAsync(new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    Reference = model.CohortRef,
                    EmployerAccountId = _hashingService.DecodeValue(model.HashedAccountId),
                    LegalEntityId = model.LegalEntityCode,
                    LegalEntityName = model.LegalEntityName,
                    ProviderId = long.Parse(model.ProviderId),
                    ProviderName = model.ProviderName,
                    CommitmentStatus = CommitmentStatus.Active,
                    EditStatus = EditStatus.ProviderOnly
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

        public async Task<FinishEditingViewModel> GetFinishEditingViewModel(string hashedAccountId, string hashedCommitmentId)
        {
            var response = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                AccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId)
            });

            var viewmodel = new FinishEditingViewModel
            {
                HashedAccountId = hashedAccountId,
                HashedCommitmentId = hashedCommitmentId,
                ApproveAndSend = response.Commitment.AgreementStatus != AgreementStatus.ProviderAgreed
            };

            return viewmodel;
        }

        public async Task ApproveCommitment(string hashedAccountId, string hashedCommitmentId, string saveOrSend)
        {
            await _mediator.SendAsync(new SubmitCommitmentCommand
            {
                EmployerAccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId),
                Message = string.Empty,
                SaveOrSend = saveOrSend
            });
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

        public async Task SubmitCommitment(SubmitCommitmentModel model)
        {
            var commitmentId = _hashingService.DecodeValue(model.HashedCommitmentId);

            if (model.SaveOrSend != "save-no-send")
            {
                await _mediator.SendAsync(new SubmitCommitmentCommand
                {
                    EmployerAccountId = _hashingService.DecodeValue(model.HashedAccountId),
                    CommitmentId = commitmentId,
                    Message = model.Message,
                    SaveOrSend = model.SaveOrSend
                });
            }
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
                Status = _statusCalculator.GetStatus(commitment.CommitmentStatus, commitment.EditStatus, commitment.Apprenticeships.Count, commitment.AgreementStatus),
                Apprenticeships = commitment.Apprenticeships?.Select(MapToApprenticeshipListItem).ToList() ?? new List<ApprenticeshipListItemViewModel>(0)
                ShowApproveOnlyOption = commitment.AgreementStatus == AgreementStatus.ProviderAgreed
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
                Status = _statusCalculator.GetStatus(commitment.CommitmentStatus, commitment.EditStatus, commitment.ApprenticeshipCount, commitment.AgreementStatus),
                ShowViewLink = commitment.EditStatus == EditStatus.EmployerOnly
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
                NINumber = apprenticeship.NINumber,
                DateOfBirthDay = apprenticeship.DateOfBirth?.Day,
                DateOfBirthMonth = apprenticeship.DateOfBirth?.Month,
                DateOfBirthYear = apprenticeship.DateOfBirth?.Year,
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
                AgreementStatus = apprenticeship.AgreementStatus,
                ProviderRef = apprenticeship.ProviderRef,
                EmployerRef = apprenticeship.EmployerRef
            };
        }

        private ApprenticeshipListItemViewModel MapToApprenticeshipListItem(Apprenticeship apprenticeship)
        {
            return new ApprenticeshipListItemViewModel
            {
                HashedId = _hashingService.HashValue(apprenticeship.Id),
                ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}",
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate
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
                DateOfBirth = GetDateTime(viewModel.DateOfBirthDay, viewModel.DateOfBirthMonth, viewModel.DateOfBirthYear),
                NINumber = viewModel.NINumber,
                ULN = viewModel.ULN,
                Cost = viewModel.Cost == null ? default(decimal?) : decimal.Parse(viewModel.Cost),
                StartDate = GetDateTime(viewModel.StartMonth, viewModel.StartYear),
                EndDate = GetDateTime(viewModel.EndMonth, viewModel.EndYear),
                ProviderRef = viewModel.ProviderRef,
                EmployerRef = viewModel.EmployerRef
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
            return (await GetTrainingProgrammes()).Where(x => x.Id == trainingCode).Single();
        }

        private DateTime? GetDateTime(int? month, int? year)
        {
            if (month.HasValue && year.HasValue)
                return new DateTime(year.Value, month.Value, 1);

            return null;
        }

        private DateTime? GetDateTime(int? day, int? month, int? year)
        {
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                DateTime dateOfBirthOut;
                if (DateTime.TryParseExact(
                    $"{year.Value}-{month.Value}-{day.Value}",
                    "yyyy-M-d",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfBirthOut))
                {
                    return dateOfBirthOut;
                }
            }

            return null;
        }

        private async Task<List<ITrainingProgramme>> GetTrainingProgrammes()
        {
            var standardsTask = _mediator.SendAsync(new GetStandardsQueryRequest());
            var frameworksTask = _mediator.SendAsync(new GetFrameworksQueryRequest());

            await Task.WhenAll(standardsTask, frameworksTask);

            return standardsTask.Result.Standards.Union(frameworksTask.Result.Frameworks.Cast<ITrainingProgramme>())
                .OrderBy(m => m.Title)
                .ToList();
        }
    }
}