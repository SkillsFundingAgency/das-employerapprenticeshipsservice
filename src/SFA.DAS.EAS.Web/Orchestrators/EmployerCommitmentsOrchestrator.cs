using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using Newtonsoft.Json;
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
using SFA.DAS.EAS.Web.Exceptions;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Validators;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.EAS.Web.Models.Types;

namespace SFA.DAS.EAS.Web.Orchestrators
{

    using Tasks.Api.Types.Templates;

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

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAll(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting all Commitments for Account: {accountId}");

            var data = await _mediator.SendAsync(new GetCommitmentsQuery
            {
                AccountId = accountId
            });

            var tasks = await _mediator.SendAsync(new GetTasksQueryRequest
            {
                AccountId = accountId
            });

            return new OrchestratorResponse<CommitmentListViewModel>
            {
                Data = new CommitmentListViewModel
                {
                    AccountHashId = hashedAccountId,
                    Commitments = data.Commitments.Select(x => MapFrom(x)).ToList(),
                    NumberOfTasks = tasks.Tasks.Count
                }
            };
        }

        public async Task<OrchestratorResponse<IList<LegalEntity>>> GetLegalEntities(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting list of Legal Entities for Account: {accountId}");

            var legalEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedLegalEntityId = hashedAccountId,
                UserId = externalUserId,
                SignedOnly = false //TODO: This should be true when signed agreements is being used
            });

            return new OrchestratorResponse<IList<LegalEntity>>
            {
                Data = legalEntities.Entites.LegalEntityList
            };
        }

        public async Task<OrchestratorResponse<CreateCommitmentViewModel>> CreateSummary(string hashedAccountId, string legalEntityCode, string providerId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting Commitment Summary Model for Account: {accountId}, LegalEntity: {legalEntityCode}, Provider: {providerId}");

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

        public async Task<string> CreateEmployerAssignedCommitment(CreateCommitmentViewModel model)
        {
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            _logger.Info($"Creating Employer assigned commitment. AccountId: {accountId}, Provider: {model.ProviderId}");

            var response = await _mediator.SendAsync(new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    Reference = model.CohortRef,
                    EmployerAccountId = accountId,
                    LegalEntityId = model.LegalEntityCode,
                    LegalEntityName = model.LegalEntityName,
                    ProviderId = model.ProviderId,
                    ProviderName = model.ProviderName,
                    CommitmentStatus = CommitmentStatus.New,
                    EditStatus = EditStatus.EmployerOnly
                }
            });

            return _hashingService.HashValue(response.CommitmentId);
        }

        public async Task<string> CreateProviderAssignedCommitment(SubmitCommitmentModel model)
        {
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            _logger.Info($"Creating Provider assigned Commitment. AccountId: {accountId}, Provider: {model.ProviderId}");

            var response = await _mediator.SendAsync(new CreateCommitmentCommand
            {
                Message = model.Message,
                Commitment = new Commitment
                {
                    Reference = model.CohortRef,
                    EmployerAccountId = accountId,
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
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(model.HashedApprenticeshipId);
            _logger.Info($"Approving Apprenticeship, Account: {accountId}, ApprenticeshipId: {model.HashedApprenticeshipId}");

            await _mediator.SendAsync(new ApproveApprenticeshipCommand
            {
                EmployerAccountId = accountId,
                CommitmentId = _hashingService.DecodeValue(model.HashedCommitmentId),
                ApprenticeshipId = apprenticeshipId
            });
        }

        public async Task<CommitmentViewModel> GetCommitmentCheckState(string hashedAccountId, string hashedCommitmentId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Commitment, Account: {accountId}, CommitmentId: {commitmentId}");

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                AccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId)
            });

            AssertCommitmentStatus(data.Commitment, EditStatus.EmployerOnly);
            AssertCommitmentStatus(data.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

            return MapFrom(data.Commitment);
        }

        public async Task<CommitmentViewModel> GetCommitment(string hashedAccountId, string hashedCommitmentId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Commitment, Account: {accountId}, CommitmentId: {commitmentId}");

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                AccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId)
            });

            return MapFrom(data.Commitment);
        }

        public async Task<CommitmentDetailsViewModel> GetCommitmentDetails(string hashedAccountId, string hashedCommitmentId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Commitment Details, Account: {accountId}, CommitmentId: {commitmentId}");

            var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                AccountId = accountId,
                CommitmentId = commitmentId
            });

            AssertCommitmentStatus(data.Commitment, EditStatus.EmployerOnly);
            AssertCommitmentStatus(data.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

            string message = await GetLatestMessage(accountId, commitmentId);
            var apprenticships = data.Commitment.Apprenticeships?.Select(MapToApprenticeshipListItem).ToList() ?? new List<ApprenticeshipListItemViewModel>(0);

            var viewModel = new CommitmentDetailsViewModel
            {
                HashedId = _hashingService.HashValue(data.Commitment.Id),
                Name = data.Commitment.Reference,
                LegalEntityName = data.Commitment.LegalEntityName,
                ProviderName = data.Commitment.ProviderName,
                Status = _statusCalculator.GetStatus(data.Commitment.EditStatus, data.Commitment.Apprenticeships.Count, data.Commitment.LastAction, data.Commitment.AgreementStatus),
                HasApprenticeships = apprenticships.Count > 0,
                Apprenticeships = apprenticships,
                ShowApproveOnlyOption = data.Commitment.AgreementStatus == AgreementStatus.ProviderAgreed,
                LatestMessage = message
            };

            return viewModel;
        }

        public async Task<ExtendedApprenticeshipViewModel> GetApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            await AssertCommitmentStatus(commitmentId, accountId);

            _logger.Info($"Getting Apprenticeship, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
            {
                AccountId = accountId,
                CommitmentId = commitmentId,
                ApprenticeshipId = apprenticeshipId
            });

            var apprenticeship = MapFrom(data.Apprenticeship);

            apprenticeship.HashedAccountId = hashedAccountId;

            // TODO: Validation errors to be used in a future story.
            var approvalValidator = new ApprenticeshipViewModelApproveValidator();

            return new ExtendedApprenticeshipViewModel
            {
                Apprenticeship = apprenticeship,
                ApprenticeshipProgrammes = await GetTrainingProgrammes(),
                ApprovalValidation = approvalValidator.Validate(apprenticeship)
            };
        }

        public async Task<FinishEditingViewModel> GetFinishEditingViewModel(string hashedAccountId, string hashedCommitmentId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Finish Editing Model, Account: {accountId}, CommitmentId: {commitmentId}");

            var response = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                AccountId = accountId,
                CommitmentId = commitmentId
            });

            AssertCommitmentStatus(response.Commitment, EditStatus.EmployerOnly);
            AssertCommitmentStatus(response.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

            var viewmodel = new FinishEditingViewModel
            {
                HashedAccountId = hashedAccountId,
                HashedCommitmentId = hashedCommitmentId,
                NotReadyForApproval = !response.Commitment.CanBeApproved,
                ApprovalState = GetApprovalState(response.Commitment),
                HasApprenticeships = response.Commitment.Apprenticeships.Any(),
                InvalidApprenticeshipCount = response.Commitment.Apprenticeships.Count(x => !x.CanBeApproved)
            };

            return viewmodel;
        }

        public async Task ApproveCommitment(string hashedAccountId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Approving Commitment, Account: {accountId}, CommitmentId: {commitmentId}");

            var lastAction = saveStatus == SaveStatus.AmendAndSend 
                ? LastAction.Amend
                : LastAction.Approve;

            await _mediator.SendAsync(new SubmitCommitmentCommand
            {
                EmployerAccountId = accountId,
                CommitmentId = commitmentId,
                Message = string.Empty,
                LastAction = lastAction,
                CreateTask = saveStatus != SaveStatus.Approve
            });
        }

        public async Task CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            var accountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId);
            var commitmentId = _hashingService.DecodeValue(apprenticeship.HashedCommitmentId);
            await AssertCommitmentStatus(commitmentId, accountId);

            _logger.Info($"Creating Apprenticeship, Account: {accountId}, CommitmentId: {commitmentId}");

            await _mediator.SendAsync(new CreateApprenticeshipCommand
            {
                AccountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId),
                Apprenticeship = await MapFrom(apprenticeship)
            });
        }

        public async Task UpdateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            var accountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(apprenticeship.HashedCommitmentId);
            var commitmentId= _hashingService.DecodeValue(apprenticeship.HashedCommitmentId);

            await AssertCommitmentStatus(commitmentId, accountId);

            _logger.Info($"Updating Apprenticeship, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await _mediator.SendAsync(new UpdateApprenticeshipCommand
            {
                AccountId = accountId,
                Apprenticeship = await MapFrom(apprenticeship)
            });
        }

        public async Task<ExtendedApprenticeshipViewModel> GetSkeletonApprenticeshipDetails(string hashedAccountId, string hashedCommitmentId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            await AssertCommitmentStatus(commitmentId, accountId);

            _logger.Info($"Getting skeleton apprenticeship model, Account: {accountId}, Commitment: {commitmentId}");

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
            if (model.SaveStatus != SaveStatus.Save)
            {
                var accountId = _hashingService.DecodeValue(model.HashedAccountId);
                var commitmentId = _hashingService.DecodeValue(model.HashedCommitmentId);
                _logger.Info($"Submiting Commitment, Account: {accountId}, Commitment: {commitmentId}, Action: {model.SaveStatus}");

                var lastAction = model.SaveStatus == SaveStatus.AmendAndSend
                    ? LastAction.Amend
                    : LastAction.Approve;

                await _mediator.SendAsync(new SubmitCommitmentCommand
                {
                    EmployerAccountId = _hashingService.DecodeValue(model.HashedAccountId),
                    CommitmentId = commitmentId,
                    Message = model.Message,
                    LastAction = lastAction,
                    CreateTask = model.SaveStatus != SaveStatus.Approve
                });
            }
        }

        public async Task PauseApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            _logger.Info($"Pausing Apprenticeship, Account: {accountId}, Apprenticeship: {apprenticeshipId}");

            await _mediator.SendAsync(new PauseApprenticeshipCommand
            {
                EmployerAccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId),
                ApprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId)
            });
        }

        public async Task ResumeApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            _logger.Info($"Resume Apprenticeship, Account: {accountId}, Apprenticeship: {apprenticeshipId}");

            await _mediator.SendAsync(new ResumeApprenticeshipCommand
            {
                EmployerAccountId = _hashingService.DecodeValue(hashedAccountId),
                CommitmentId = _hashingService.DecodeValue(hashedCommitmentId),
                ApprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId)
            });
        }

        public async Task<List<Provider>> GetProvider(int providerId)
        {
            _logger.Info($"Getting Provider Details, Provider: {providerId}");

            var data = await _mediator.SendAsync(new GetProviderQueryRequest
            {
                ProviderId = providerId
            });

            return data?.ProvidersView?.Providers;
        }

        private static ApprovalState GetApprovalState(Commitment commitment)
        {
            if (!commitment.Apprenticeships.Any()) return ApprovalState.ApproveAndSend;

            var approvalState = commitment.Apprenticeships.Any(m => m.AgreementStatus == AgreementStatus.NotAgreed
                                || m.AgreementStatus == AgreementStatus.EmployerAgreed) ? ApprovalState.ApproveAndSend : ApprovalState.ApproveOnly;
 
            return approvalState;
         }
 
        private async Task<string> GetLatestMessage(long accountId, long commitmentId)
        {
            var allTasks = await _mediator.SendAsync(new GetTasksQueryRequest { AccountId = accountId });

            var taskForCommitment = allTasks?.Tasks
                .Select(x => new { Task = JsonConvert.DeserializeObject<CreateCommitmentTemplate>(x.Body), CreateDate = x.CreatedOn })
                .Where(x => x.Task != null && x.Task.CommitmentId == commitmentId)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefault();

            var message = taskForCommitment?.Task?.Message ?? string.Empty;

            return message;
        }

        private async Task<GetProvidersQueryResponse> GetProviders()
        {
            return await _mediator.SendAsync(new GetProvidersQueryRequest());
        }

        private async Task<GetAccountLegalEntitiesResponse> GetActiveLegalEntities(string hashedAccountId, string externalUserId)
        {
            return await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedLegalEntityId = hashedAccountId,
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
                ProviderName = commitment.ProviderName
            };
        }

        private CommitmentListItemViewModel MapFrom(CommitmentListItem commitment)
        {
            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Status = _statusCalculator.GetStatus(commitment.EditStatus, commitment.ApprenticeshipCount, commitment.LastAction, commitment.AgreementStatus),
                ShowViewLink = commitment.EditStatus == EditStatus.EmployerOnly
            };
        }

        private ApprenticeshipViewModel MapFrom(Apprenticeship apprenticeship)
        {
            return new ApprenticeshipViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                HashedCommitmentId = _hashingService.HashValue(apprenticeship.CommitmentId),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                NINumber = apprenticeship.NINumber,
                DateOfBirth = new DateTimeViewModel(apprenticeship.DateOfBirth?.Day, apprenticeship.DateOfBirth?.Month, apprenticeship.DateOfBirth?.Year),
                ULN = apprenticeship.ULN,
                TrainingType = apprenticeship.TrainingType,
                TrainingId = apprenticeship.TrainingCode,
                TrainingName = apprenticeship.TrainingName,
                Cost = NullableDecimalToString(apprenticeship.Cost),
                StartDate = new DateTimeViewModel(apprenticeship.StartDate),
                EndDate = new DateTimeViewModel(apprenticeship.EndDate),
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
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                ApprenticeName = apprenticeship.ApprenticeshipName,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                CanBeApproved = apprenticeship.CanBeApproved
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
                Id = string.IsNullOrWhiteSpace(viewModel.HashedApprenticeshipId) ? 0L : _hashingService.DecodeValue(viewModel.HashedApprenticeshipId),
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DateOfBirth = viewModel.DateOfBirth.DateTime,
                NINumber = viewModel.NINumber,
                ULN = viewModel.ULN,
                Cost = viewModel.Cost == null ? default(decimal?) : decimal.Parse(viewModel.Cost),
                StartDate = viewModel.StartDate.DateTime,
                EndDate = viewModel.EndDate.DateTime,
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


        private async Task<List<ITrainingProgramme>> GetTrainingProgrammes()
        {
            var standardsTask = _mediator.SendAsync(new GetStandardsQueryRequest());
            var frameworksTask = _mediator.SendAsync(new GetFrameworksQueryRequest());

            await Task.WhenAll(standardsTask, frameworksTask);

            return standardsTask.Result.Standards.Union(frameworksTask.Result.Frameworks.Cast<ITrainingProgramme>())
                .OrderBy(m => m.Title)
                .ToList();
        }

        private async Task AssertCommitmentStatus(long commitmentId, long accountId)
        {
            var commitmentData = await _mediator.SendAsync(new GetCommitmentQueryRequest
            {
                AccountId = accountId,
                CommitmentId = commitmentId
            });
            AssertCommitmentStatus(commitmentData.Commitment, EditStatus.EmployerOnly);
            AssertCommitmentStatus(commitmentData.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);
        }

        private static void AssertCommitmentStatus(
            Commitment commitment,
            params AgreementStatus[] allowedAgreementStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedAgreementStatuses.Contains(commitment.AgreementStatus))
                throw new InvalidStateException($"Invalid commitment state (agreement status is {commitment.AgreementStatus}, expected {string.Join(",", allowedAgreementStatuses)})");
        }

        private static void AssertCommitmentStatus(Commitment commitment, params EditStatus[] allowedEditStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedEditStatuses.Contains(commitment.EditStatus))
                throw new InvalidStateException($"Invalid commitment state (edit status is {commitment.EditStatus}, expected {string.Join(",", allowedEditStatuses)})");
        }
    }
}
