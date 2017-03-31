using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.CreateApprenticeship;
using SFA.DAS.EAS.Application.Commands.CreateCommitment;
using SFA.DAS.EAS.Application.Commands.DeleteApprentice;
using SFA.DAS.EAS.Application.Commands.DeleteCommitment;
using SFA.DAS.EAS.Application.Commands.SubmitCommitment;
using SFA.DAS.EAS.Application.Commands.UpdateApprenticeship;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Application.Queries.GetCommitment;
using SFA.DAS.EAS.Application.Queries.GetCommitments;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityAgreement;
using SFA.DAS.EAS.Application.Queries.GetProvider;
using SFA.DAS.EAS.Application.Queries.GetTasks;
using SFA.DAS.EAS.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Exceptions;
using SFA.DAS.Tasks.Api.Types.Templates;
using System.Net;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.EAS.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EAS.Web.Enums;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebGrease.Css.Extensions;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public sealed class EmployerCommitmentsOrchestrator : CommitmentsBaseOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly ILogger _logger;
        private readonly ICommitmentStatusCalculator _statusCalculator;

        private readonly Func<int, string> _addPluralizationSuffix = i => i > 1 ? "s" : "";
        private readonly Func<CommitmentListItem, Task<string>> _latestMessageFromProviderFunc;
        private readonly Func<CommitmentListItem, Task<string>> _latestMessageFromEmployerFunc;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly ICommitmentMapper _commitmentMapper;

        public EmployerCommitmentsOrchestrator(
            IMediator mediator, 
            IHashingService hashingService, 
            ICommitmentStatusCalculator statusCalculator,
            IApprenticeshipMapper apprenticeshipMapper,
            ICommitmentMapper commitmentMapper,
            ILogger logger) : base(mediator, hashingService, logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (statusCalculator == null)
                throw new ArgumentNullException(nameof(statusCalculator));
            if (apprenticeshipMapper == null)
                throw new ArgumentNullException(nameof(apprenticeshipMapper));
            if (commitmentMapper == null)
                throw new ArgumentNullException(nameof(commitmentMapper));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _hashingService = hashingService;
            _statusCalculator = statusCalculator;
            _apprenticeshipMapper = apprenticeshipMapper;
            _commitmentMapper = commitmentMapper;
            _logger = logger;

            _latestMessageFromProviderFunc = async item => await GetLatestMessageFromProvider(item.EmployerAccountId, item.Id);
            _latestMessageFromEmployerFunc = async item => await GetLatestMessageFromEmployer(item.ProviderId, item.Id);
        }

        public async Task<OrchestratorResponse<Account>> CheckAccountAuthorization(string hashedAccountId, string externalUserId)
        {
            return await CheckUserAuthorization(() =>
            {
                return Task.FromResult(new OrchestratorResponse<Account>
                {
                    Status = HttpStatusCode.OK
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<CommitmentInformViewModel>> GetInform(string hashedAccountId, string externalUserId)
        {
            return await CheckUserAuthorization(() =>
            {
                return Task.FromResult(new OrchestratorResponse<CommitmentInformViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new CommitmentInformViewModel
                    {
                        HashedAccountId = hashedAccountId
                    }
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<SelectProviderViewModel>> GetProviderSearch(string hashedAccountId, string externalUserId, string legalEntityCode, string cohortRef)
        {
            return await CheckUserAuthorization(() =>
            {
                return Task.FromResult(new OrchestratorResponse<SelectProviderViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = new SelectProviderViewModel
                    {
                        LegalEntityCode = legalEntityCode,
                        CohortRef = cohortRef
                    }
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<SelectLegalEntityViewModel>> GetLegalEntities(string hashedAccountId, string cohortRef, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting list of Legal Entities for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var legalEntities = await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
                {
                    HashedLegalEntityId = hashedAccountId,
                    UserId = externalUserId,
                    SignedOnly = false //TODO: This should be true when signed agreements is being used
                });

                return new OrchestratorResponse<SelectLegalEntityViewModel>
                {
                    Data = new SelectLegalEntityViewModel
                    {
                        CohortRef = string.IsNullOrWhiteSpace(cohortRef) ? CreateReference() : cohortRef,
                        LegalEntities = legalEntities.Entites.LegalEntityList
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ConfirmProviderViewModel>> GetProvider(string hashedAccountId, string externalUserId, SelectProviderViewModel model)
        {
            var providerId = int.Parse(model.ProviderId);

            return await GetProvider(hashedAccountId, externalUserId, providerId, model.LegalEntityCode, model.CohortRef);
        }

        public async Task<OrchestratorResponse<ConfirmProviderViewModel>> GetProvider(string hashedAccountId, string externalUserId, ConfirmProviderViewModel model)
        {
            return await GetProvider(hashedAccountId, externalUserId, model.ProviderId, model.LegalEntityCode, model.CohortRef);
        }

        private Task<OrchestratorResponse<ConfirmProviderViewModel>> GetProvider(string hashedAccountId, string externalUserId, int providerId, string legalEntityCode, string cohortRef)
        {
            _logger.Info($"Getting Provider Details, Provider: {providerId}");

            return CheckUserAuthorization(async () =>
            {
                var provider = await ProviderSearch(providerId);

                return new OrchestratorResponse<ConfirmProviderViewModel>
                {
                    Data = new ConfirmProviderViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        LegalEntityCode = legalEntityCode,
                        ProviderId = providerId,
                        Provider = provider,
                        CohortRef = cohortRef,
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        private async Task<Provider> ProviderSearch(long providerId)
        {
            var response =  await _mediator.SendAsync(new GetProviderQueryRequest
            {
                ProviderId = providerId
            });

            return response.ProvidersView?.Provider;
        }

        public async Task<OrchestratorResponse<CreateCommitmentViewModel>> CreateSummary(string hashedAccountId, string legalEntityCode, string providerId, string cohortRef, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting Commitment Summary Model for Account: {accountId}, LegalEntity: {legalEntityCode}, Provider: {providerId}");

            return await CheckUserAuthorization(async () =>
            {
                var provider = await ProviderSearch(int.Parse(providerId));

                var legalEntities = await GetActiveLegalEntities(hashedAccountId, externalUserId);
                var legalEntity = legalEntities.Entites.LegalEntityList.Single(x => x.Code.Equals(legalEntityCode, StringComparison.InvariantCultureIgnoreCase));

                return new OrchestratorResponse<CreateCommitmentViewModel>
                {
                    Data = new CreateCommitmentViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        LegalEntityCode = legalEntityCode,
                        LegalEntityName = legalEntity.Name,
                        LegalEntityAddress = legalEntity.RegisteredAddress,
                        LegalEntitySource = legalEntity.Source,
                        ProviderId = provider.Ukprn,
                        ProviderName = provider.ProviderName,
                        CohortRef = cohortRef
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<string>> CreateEmployerAssignedCommitment(CreateCommitmentViewModel model, string externalUserId, string userDisplayName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            _logger.Info($"Creating Employer assigned commitment. AccountId: {accountId}, Provider: {model.ProviderId}");

            return await CheckUserAuthorization(async () =>
            {
                var response = await _mediator.SendAsync(new CreateCommitmentCommand
                {
                    Commitment = new Commitment
                    {
                        Reference = model.CohortRef,
                        EmployerAccountId = accountId,
                        LegalEntityId = model.LegalEntityCode,
                        LegalEntityName = model.LegalEntityName,
                        LegalEntityAddress = model.LegalEntityAddress,
                        LegalEntityOrganisationType = (OrganisationType) model.LegalEntitySource,
                        ProviderId = model.ProviderId,
                        ProviderName = model.ProviderName,
                        CommitmentStatus = CommitmentStatus.New,
                        EditStatus = EditStatus.EmployerOnly,
                        EmployerLastUpdateInfo = new LastUpdateInfo { Name = userDisplayName, EmailAddress = userEmail }
                    },
                    UserId = externalUserId
                });

                return new OrchestratorResponse<string>
                {
                    Data = _hashingService.HashValue(response.CommitmentId)
                };

            }, model.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<string>> CreateProviderAssignedCommitment(SubmitCommitmenViewModel model, string externalUserId, string userDisplayName, string userEmail)
        {
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            _logger.Info($"Creating Provider assigned Commitment. AccountId: {accountId}, Provider: {model.ProviderId}");

            return await CheckUserAuthorization(async () => 
            {
                var response = await _mediator.SendAsync(new CreateCommitmentCommand
                {
                    Message = model.Message,
                    Commitment = new Commitment
                    {
                        Reference = model.CohortRef,
                        EmployerAccountId = accountId,
                        LegalEntityId = model.LegalEntityCode,
                        LegalEntityName = model.LegalEntityName,
                        LegalEntityAddress = model.LegalEntityAddress,
                        LegalEntityOrganisationType = (OrganisationType)model.LegalEntitySource,
                        ProviderId = long.Parse(model.ProviderId),
                        ProviderName = model.ProviderName,
                        CommitmentStatus = CommitmentStatus.Active,
                        EditStatus = EditStatus.ProviderOnly,
                        EmployerLastUpdateInfo = new LastUpdateInfo { Name = userDisplayName, EmailAddress = userEmail },
                    },
                    UserId = externalUserId
                });

                return new OrchestratorResponse<string>
                {
                    Data = _hashingService.HashValue(response.CommitmentId)
                };
                
            }, model.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetSkeletonApprenticeshipDetails(string hashedAccountId, string externalUserId, string hashedCommitmentId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting skeleton apprenticeship model, Account: {accountId}, Commitment: {commitmentId}");

            return await CheckUserAuthorization(async () =>
            {
                await AssertCommitmentStatus(commitmentId, accountId);

                var apprenticeship = new ApprenticeshipViewModel
                {
                    HashedAccountId = hashedAccountId,
                    HashedCommitmentId = hashedCommitmentId,
                };

                return new OrchestratorResponse<ExtendedApprenticeshipViewModel>
                {
                    Data = new ExtendedApprenticeshipViewModel
                    {
                        Apprenticeship = apprenticeship,
                        ApprenticeshipProgrammes = await GetTrainingProgrammes()
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task CreateApprenticeship(ApprenticeshipViewModel apprenticeship, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId);
            var commitmentId = _hashingService.DecodeValue(apprenticeship.HashedCommitmentId);
            _logger.Info($"Creating Apprenticeship, Account: {accountId}, CommitmentId: {commitmentId}");

            await CheckUserAuthorization(async () => 
            {
                await AssertCommitmentStatus(commitmentId, accountId);

                await _mediator.SendAsync(new CreateApprenticeshipCommand
                {
                    AccountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId),
                    Apprenticeship = await _apprenticeshipMapper.MapFromAsync(apprenticeship),
                    UserId = externalUserId
                });
            }, apprenticeship.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetApprenticeship(string hashedAccountId, string externalUserId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Apprenticeship, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () => 
            {
                await AssertCommitmentStatus(commitmentId, accountId);

                var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId
                });

                var apprenticeship = _apprenticeshipMapper.MapToApprenticeshipViewModel(data.Apprenticeship);

                apprenticeship.HashedAccountId = hashedAccountId;

                var overlaps = await _mediator.SendAsync(
                    new GetOverlappingApprenticeshipsQueryRequest
                        {
                            Apprenticeship = new[] { data.Apprenticeship }
                        });

                return new OrchestratorResponse<ExtendedApprenticeshipViewModel>
                {
                    Data = new ExtendedApprenticeshipViewModel
                    {
                        Apprenticeship = apprenticeship,
                        ApprenticeshipProgrammes = await GetTrainingProgrammes(),
                        ValidationErrors = _apprenticeshipMapper.MapOverlappingErrors(overlaps)
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task UpdateApprenticeship(ApprenticeshipViewModel apprenticeship, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(apprenticeship.HashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(apprenticeship.HashedCommitmentId);
            var commitmentId = _hashingService.DecodeValue(apprenticeship.HashedCommitmentId);
            _logger.Info($"Updating Apprenticeship, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await CheckUserAuthorization(async () => 
            {
                await AssertCommitmentStatus(commitmentId, accountId);

                await _mediator.SendAsync(new UpdateApprenticeshipCommand
                {
                    AccountId = accountId,
                    Apprenticeship = await _apprenticeshipMapper.MapFromAsync(apprenticeship),
                    UserId = externalUserId
                });
            }, apprenticeship.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<FinishEditingViewModel>> GetFinishEditingViewModel(string hashedAccountId, string externalUserId, string hashedCommitmentId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Finish Editing Model, Account: {accountId}, CommitmentId: {commitmentId}");

            return await CheckUserAuthorization(async () =>
            {
                var response = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId
                });

                AssertCommitmentStatus(response.Commitment, EditStatus.EmployerOnly);
                AssertCommitmentStatus(response.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

                var agreementResponse = await _mediator.SendAsync(new GetLegalEntityAgreementRequest
                {
                    AccountId = accountId,
                    LegalEntityCode = response.Commitment.LegalEntityId
                });

                var hasSigned = agreementResponse.EmployerAgreement == null;

                var overlaps = await _mediator.SendAsync(
                    new GetOverlappingApprenticeshipsQueryRequest
                    {
                        Apprenticeship = response.Commitment.Apprenticeships
                    });

                return new OrchestratorResponse<FinishEditingViewModel>
                {
                    Data = new FinishEditingViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        HashedCommitmentId = hashedCommitmentId,
                        NotReadyForApproval = !response.Commitment.CanBeApproved,
                        ApprovalState = GetApprovalState(response.Commitment),
                        HasApprenticeships = response.Commitment.Apprenticeships.Any(),
                        InvalidApprenticeshipCount = response.Commitment.Apprenticeships.Count(x => !x.CanBeApproved),
                        HasSignedTheAgreement = hasSigned,
                        HasOverlappingErrors = overlaps?.Overlaps?.Any() ?? false
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task ApproveCommitment(string hashedAccountId, string externalUserId, string userDisplayName, string userEmail, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Approving Commitment, Account: {accountId}, CommitmentId: {commitmentId}");

            await CheckUserAuthorization(async () => 
            {
                var lastAction = saveStatus == SaveStatus.AmendAndSend
                    ? LastAction.Amend
                    : LastAction.Approve;

                await _mediator.SendAsync(new SubmitCommitmentCommand
                {
                    EmployerAccountId = accountId,
                    CommitmentId = commitmentId,
                    HashedCommitmentId = hashedAccountId,
                    Message = string.Empty,
                    LastAction = lastAction,
                    UserDisplayName = userDisplayName,
                    UserEmailAddress = userEmail,
                    CreateTask = saveStatus != SaveStatus.Approve,
                    UserId = externalUserId
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<SubmitCommitmentViewModel>> GetSubmitNewCommitmentModel(string hashedAccountId, string externalUserId, string legalEntityCode, string legalEntityName, string legalEntityAddress, short legalEntitySource, string providerId, string providerName, string cohortRef, SaveStatus saveStatus)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting Submit New Commitment ViewModel, Account: {accountId}");

            return await CheckUserAuthorization(() => 
            {
                return new OrchestratorResponse<SubmitCommitmentViewModel>
                {
                    Data = new SubmitCommitmentViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        LegalEntityCode = legalEntityCode,
                        LegalEntityName = legalEntityName,
                        LegalEntityAddress = legalEntityAddress,
                        LegalEntitySource = legalEntitySource,
                        ProviderId = long.Parse(providerId),
                        ProviderName = providerName,
                        CohortRef = cohortRef,
                        SaveStatus = saveStatus
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<SubmitCommitmentViewModel>> GetSubmitCommitmentModel(string hashedAccountId, string externalUserId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Submit Existing Commitment ViewModel, Account: {accountId}, CommitmentId: {commitmentId}");

            return await CheckUserAuthorization(async () => 
            {
                var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = _hashingService.DecodeValue(hashedAccountId),
                    CommitmentId = _hashingService.DecodeValue(hashedCommitmentId)
                });

                AssertCommitmentStatus(data.Commitment, EditStatus.EmployerOnly);
                AssertCommitmentStatus(data.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

                var commitment = _commitmentMapper.MapToCommitmentViewModel(data.Commitment);

                return new OrchestratorResponse<SubmitCommitmentViewModel>
                {
                    Data = new SubmitCommitmentViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        HashedCommitmentId = hashedCommitmentId,
                        ProviderName = commitment.ProviderName,
                        SaveStatus = saveStatus
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task SubmitCommitment(SubmitCommitmenViewModel model, string externalUserId, string userDisplayName, string userEmail)
        {
            await CheckUserAuthorization(async () => 
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
                        HashedCommitmentId = model.HashedCommitmentId,
                        Message = model.Message,
                        LastAction = lastAction,
                        UserDisplayName = userDisplayName,
                        UserEmailAddress = userEmail,
                        CreateTask = model.SaveStatus != SaveStatus.Approve,
                        UserId = externalUserId
                    });
                }
            }, model.HashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<AcknowledgementViewModel>> GetAcknowledgementModelForExistingCommitment(string hashedAccountId, string hashedCommitmentId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Acknowldedgement Model for existing commitment, Account: {accountId}, CommitmentId: {commitmentId}");

            return await CheckUserAuthorization(async () => 
            {
                var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId
                });

                var messageTask = GetLatestMessageFromEmployer(data.Commitment);

                return new OrchestratorResponse<AcknowledgementViewModel>
                {
                    Data = new AcknowledgementViewModel
                    {
                        HashedAccount = hashedAccountId,
                        HashedCommitmentId = hashedCommitmentId,
                        ProviderName = data.Commitment.ProviderName,
                        LegalEntityName = data.Commitment.LegalEntityName,
                        Message = await messageTask
                    }
                };
            }, hashedAccountId, externalUserId);
        }
        
        public async Task<OrchestratorResponse<YourCohortsViewModel>> GetYourCohorts(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your cohorts for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await _mediator.SendAsync(new GetCommitmentsQuery
                {
                    AccountId = accountId
                });

                var commitmentStatuses = data.Commitments
                    .Select(m => _statusCalculator.GetStatus(
                        m.EditStatus,
                        m.ApprenticeshipCount,
                        m.LastAction,
                        m.AgreementStatus))
                    .ToList();

                return new OrchestratorResponse<YourCohortsViewModel>
                {
                    Data = new YourCohortsViewModel
                               {
                                   WaitingToBeSentCount = commitmentStatuses.Count(m => m == RequestStatus.NewRequest),
                                   ReadyForApprovalCount = commitmentStatuses.Count(m => m == RequestStatus.ReadyForApproval),
                                   ReadyForReviewCount = commitmentStatuses.Count(m => m == RequestStatus.ReadyForReview),
                                   WithProviderCount = commitmentStatuses.Count(m => 
                                        m == RequestStatus.WithProviderForApproval 
                                     || m == RequestStatus.SentToProvider
                                     || m == RequestStatus.SentForReview)
                                }
                }; 
                        
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAllWaitingToBeSent(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your cohorts waiting to be sent for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
                {
                    var commitments = (await GetAll(accountId, RequestStatus.NewRequest)).ToList();

                    return new OrchestratorResponse<CommitmentListViewModel>
                    {
                        Data = new CommitmentListViewModel
                        {
                            AccountHashId = hashedAccountId,
                            Commitments = await Task.WhenAll(commitments.Select(m => _commitmentMapper.MapToCommitmentListItemViewModelAsync(m, _latestMessageFromProviderFunc))),
                            PageTitle = "Waiting to be sent",
                            PageId = "waiting-to-be-sent",
                            PageHeading = "Waiting to be sent",
                            PageHeading2 = $"You have <strong>{commitments.Count}</strong> cohort{_addPluralizationSuffix(commitments.ToList().Count)} waiting to be sent to the training provider for review or approval.",
                        }
                    };

                }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAllReadyForApproval(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your cohorts ready for approval for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var commitments = (await GetAll(accountId, RequestStatus.ReadyForApproval)).ToList();

                return new OrchestratorResponse<CommitmentListViewModel>
                {
                    Data = new CommitmentListViewModel
                    {
                        AccountHashId = hashedAccountId,
                        Commitments = await Task.WhenAll(commitments.Select(m => _commitmentMapper.MapToCommitmentListItemViewModelAsync(m, _latestMessageFromProviderFunc))),
                        PageTitle = "Cohorts for approval",
                        PageId = "ready-for-approval",
                        PageHeading = "Cohorts for approval",
                        PageHeading2 = $"You have <strong>{commitments.Count}</strong> cohort{_addPluralizationSuffix(commitments.ToList().Count)} ready for your approval.",

                    }
                };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAllReadyForReview(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your cohorts ready for review for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var commitments = (await GetAll(accountId, RequestStatus.ReadyForReview)).ToList();

                return new OrchestratorResponse<CommitmentListViewModel>
                {
                    Data = new CommitmentListViewModel
                    {
                        AccountHashId = hashedAccountId,
                        Commitments = await Task.WhenAll(commitments.Select(m => _commitmentMapper.MapToCommitmentListItemViewModelAsync(m, _latestMessageFromProviderFunc))),
                        PageTitle = "Cohorts for review",
                        PageId = "ready-for-review",
                        PageHeading = "Cohorts for review",
                        PageHeading2 = $"You have <strong>{commitments.Count}</strong> cohort{_addPluralizationSuffix(commitments.ToList().Count)} ready for review.",

                    }
                };

            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<CommitmentListViewModel>> GetAllWithProvider(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting your cohorts with the provider sent for Account: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                var withProviderForApproval = await GetAll(accountId, RequestStatus.WithProviderForApproval);
                var sentForReview = await GetAll(accountId, RequestStatus.SentForReview);
                var sentToProvider = await GetAll(accountId, RequestStatus.SentToProvider);

                var commitments = withProviderForApproval
                                  .Concat(sentForReview)
                                  .Concat(sentToProvider)
                                  .ToList();

                return new OrchestratorResponse<CommitmentListViewModel>
                {
                    Data = new CommitmentListViewModel
                    {
                        AccountHashId = hashedAccountId,
                        Commitments = await Task.WhenAll(commitments.Select(m => _commitmentMapper.MapToCommitmentListItemViewModelAsync(m, _latestMessageFromEmployerFunc))),
                        PageTitle = "With training providers",
                        PageId = "with-the-provider",
                        PageHeading = "With training providers",
                        PageHeading2 = $"You have <strong>{commitments.Count}</strong> cohort{_addPluralizationSuffix(commitments.ToList().Count)} with training providers for them to add apprentices, or review and approve details."
                    }
                };

            }, hashedAccountId, externalUserId);
        }

        private async Task<IEnumerable<CommitmentListItem>> GetAll(long accountId, RequestStatus requestStatus)
        {
            _logger.Info($"Getting all Commitments for Account: {accountId}");

            var data = await _mediator.SendAsync(new GetCommitmentsQuery
            {
                AccountId = accountId
            });
            return data.Commitments.Where(
                            m => _statusCalculator.GetStatus(m.EditStatus, m.ApprenticeshipCount, m.LastAction, m.AgreementStatus)
                                    == requestStatus);
        }

        public async Task<OrchestratorResponse<CommitmentDetailsViewModel>> GetCommitmentDetails(string hashedAccountId, string hashedCommitmentId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);
            _logger.Info($"Getting Commitment Details, Account: {accountId}, CommitmentId: {commitmentId}");

            return await CheckUserAuthorization(async () =>
            {
                var data = await _mediator.SendAsync(new GetCommitmentQueryRequest
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId
                });

                AssertCommitmentStatus(data.Commitment, EditStatus.EmployerOnly);
                AssertCommitmentStatus(data.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

                var messageTask = GetLatestMessageFromProvider(data.Commitment);

                var overlappingApprenticeships = await _mediator.SendAsync(
                   new GetOverlappingApprenticeshipsQueryRequest
                   {
                       Apprenticeship = data.Commitment.Apprenticeships
                   });

                var apprenticships = data.Commitment.Apprenticeships?.Select(
                    a => MapToApprenticeshipListItem(a, overlappingApprenticeships)).ToList() ?? new List<ApprenticeshipListItemViewModel>(0);

                var trainingProgrammes = await GetTrainingProgrammes();

                var apprenticeshipGroups = new List<ApprenticeshipListItemGroupViewModel>();
                foreach (var group in apprenticships.OrderBy(x=> x.TrainingName).GroupBy(x => x.TrainingCode))
                {
                    apprenticeshipGroups.Add(new ApprenticeshipListItemGroupViewModel
                    {
                        Apprenticeships = group.OrderBy(x => x.CanBeApproved).ToList(),
                        TrainingProgramme = trainingProgrammes.FirstOrDefault(x => x.Id == group.Key)
                    });
                }

                var warnings = new Dictionary<string, string>();
                apprenticeshipGroups
                    .Where(m => m.ShowFundingLimitWarning)
                    .ForEach(group => warnings.Add(group.GroupId, $"Cost for {group.TrainingProgramme.Title}"));

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
                    LatestMessage = await messageTask,
                    ApprenticeshipGroups = apprenticeshipGroups,
                    HasOverlappingErrors = apprenticeshipGroups.Any(m => m.ShowOverlapError),
                    FundingCapWarnings = warnings
                };

                return new OrchestratorResponse<CommitmentDetailsViewModel>
                {
                    Data = viewModel
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<DeleteCommitmentViewModel>> GetDeleteCommitmentModel(string hashedAccountId, string hashedCommitmentId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            return await CheckUserAuthorization(
                async () =>
                    {
                        var commitmentData = await _mediator.SendAsync(new GetCommitmentQueryRequest
                        {
                            AccountId = accountId,
                            CommitmentId = commitmentId
                        });

                        AssertCommitmentStatus(commitmentData.Commitment, EditStatus.EmployerOnly);
                        AssertCommitmentStatus(commitmentData.Commitment, AgreementStatus.EmployerAgreed, AgreementStatus.ProviderAgreed, AgreementStatus.NotAgreed);

                        Func<string, string> textOrDefault = txt => !string.IsNullOrEmpty(txt) ? txt : "without training course details";
                        var programmeSummary = commitmentData.Commitment.Apprenticeships
                                .GroupBy(m => m.TrainingName) 
                                .Select(m => $"{m.Count()} {textOrDefault(m.Key)}")
                                .ToList();

                        return new OrchestratorResponse<DeleteCommitmentViewModel>
                                   {
                                       Data = new DeleteCommitmentViewModel
                                                  {
                                                       HashedAccountId = hashedAccountId,
                                                       HashedCommitmentId = hashedCommitmentId,
                                                       ProviderName = commitmentData.Commitment.ProviderName,
                                                       NumberOfApprenticeships = commitmentData.Commitment.Apprenticeships.Count,
                                                       ProgrammeSummaries = programmeSummary
                                       }
                                   };
                    }, hashedAccountId, externalUserId);
        }

        public async Task DeleteCommitment(string hashedAccountId, string hashedCommitmentId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            _logger.Info($"Deleting commitment {hashedCommitmentId} Account: {accountId}, CommitmentId: {commitmentId}");

            await CheckUserAuthorization(async () =>
            {
                await _mediator.SendAsync(new DeleteCommitmentCommand
                {
                    AccountId = accountId,
                    CommitmentId = commitmentId,
                    UserId = externalUserId
                });
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<DeleteApprenticeshipConfirmationViewModel>> GetDeleteApprenticeshipViewModel(string hashedAccountId, string externalUserId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var commitmentId = _hashingService.DecodeValue(hashedCommitmentId);

            return await CheckUserAuthorization(async () =>
            {
                var apprenticeship = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId
                });

                await AssertCommitmentStatus(commitmentId, accountId);

                return new OrchestratorResponse<DeleteApprenticeshipConfirmationViewModel>
                {
                    Data = new DeleteApprenticeshipConfirmationViewModel
                    {
                        HashedAccountId = hashedAccountId,
                        HashedCommitmentId = hashedCommitmentId,
                        HashedApprenticeshipId = hashedApprenticeshipId,
                        ApprenticeshipName = apprenticeship.Apprenticeship.ApprenticeshipName,
                        DateOfBirth = apprenticeship.Apprenticeship.DateOfBirth.HasValue ? apprenticeship.Apprenticeship.DateOfBirth.Value.ToGdsFormat() : string.Empty
                    }
                };

            }, hashedAccountId, externalUserId);

        }


        public async Task<bool> AnyCohortsForCurrentStatus(string hashedAccountId, RequestStatus requestStatusFromSession)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var data = (await GetAll(accountId, requestStatusFromSession)).ToList();
            return data.Any();
        }

        public async Task<OrchestratorResponse<LegalEntitySignedAgreementViewModel>> GetLegalEntitySignedAgreementViewModel(string hashedAccountId, string legalEntityCode, string cohortRef)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var agreementResponse = await _mediator.SendAsync(new GetLegalEntityAgreementRequest
            {
                AccountId = accountId,
                LegalEntityCode = legalEntityCode
            });

            var hasSigned = agreementResponse.EmployerAgreement == null;

            return new OrchestratorResponse<LegalEntitySignedAgreementViewModel>
            {
                Data = new LegalEntitySignedAgreementViewModel
                {
                    HashedAccountId = hashedAccountId,
                    LegalEntityCode = legalEntityCode,
                    CohortRef = cohortRef,
                    HasSignedAgreement = hasSigned,
                    LegalEntityName = agreementResponse?.EmployerAgreement?.LegalEntityName ?? string.Empty
                }
            };
        }

        public async Task<Dictionary<string, string>> ValidateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            var overlappingErrors = await _mediator.SendAsync(
                new GetOverlappingApprenticeshipsQueryRequest
                {
                    Apprenticeship = new List<Apprenticeship> { await _apprenticeshipMapper.MapFrom(apprenticeship) }
                });

            return _apprenticeshipMapper.MapOverlappingErrors(overlappingErrors);
        }

        public async Task DeleteApprenticeship(DeleteApprenticeshipConfirmationViewModel model, string externalUser)
        {
            var accountId = _hashingService.DecodeValue(model.HashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(model.HashedApprenticeshipId);

            await CheckUserAuthorization(async () =>
                    {
                        await _mediator.SendAsync(new GetApprenticeshipQueryRequest
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId
                        });

                        await _mediator.SendAsync(new DeleteApprenticeshipCommand
                        {
                            AccountId = accountId,
                            ApprenticeshipId = apprenticeshipId,
                            UserId = externalUser
                        });

                    }, model.HashedAccountId, externalUser);
        }

        private static string CreateReference()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }

        private static ApprovalState GetApprovalState(Commitment commitment)
        {
            if (!commitment.Apprenticeships.Any()) return ApprovalState.ApproveAndSend;

            var approvalState = commitment.Apprenticeships.Any(m => m.AgreementStatus == AgreementStatus.NotAgreed
                                || m.AgreementStatus == AgreementStatus.EmployerAgreed) ? ApprovalState.ApproveAndSend : ApprovalState.ApproveOnly;
 
            return approvalState;
         }

        private async Task<string> GetLatestMessageFromEmployer(Commitment commitment)
        {
            return await GetLatestMessage(commitment.ProviderId, commitment.Id, false);
        }

        private async Task<string> GetLatestMessageFromEmployer(long? providerId, long commitmentId)
        {
            return await GetLatestMessage(providerId, commitmentId, false);
        }
        private async Task<string> GetLatestMessageFromProvider(Commitment commitment)
        {
            return await GetLatestMessage(commitment.EmployerAccountId, commitment.Id, true);
        }

        private async Task<string> GetLatestMessageFromProvider(long? employerAccountId, long commitmentId)
        {
            return await GetLatestMessage(employerAccountId, commitmentId, true);
        }

        private async Task<string> GetLatestMessage(long? accountId, long commitmentId, bool assigneeEmployer)
        {
            if (accountId == null) return string.Empty;
            var allTasks = await _mediator.SendAsync(new GetTasksQueryRequest { AccountId = accountId.Value, AssigneeEmployer = assigneeEmployer });

            var taskForCommitment = allTasks?.Tasks
                .Select(x => new { Task = JsonConvert.DeserializeObject<CreateCommitmentTemplate>(x.Body), CreateDate = x.CreatedOn })
                .Where(x => x.Task != null && x.Task.CommitmentId == commitmentId)
                .OrderByDescending(x => x.CreateDate)
                .FirstOrDefault();

            var message = taskForCommitment?.Task?.Message ?? string.Empty;

            return message;
        }

        private async Task<GetAccountLegalEntitiesResponse> GetActiveLegalEntities(string hashedAccountId, string externalUserId)
        {
            return await _mediator.SendAsync(new GetAccountLegalEntitiesRequest
            {
                HashedLegalEntityId = hashedAccountId,
                UserId = externalUserId
            });
        }

        private ApprenticeshipListItemViewModel MapToApprenticeshipListItem(Apprenticeship apprenticeship, GetOverlappingApprenticeshipsQueryResponse overlappingApprenticeships)
        {
            return new ApprenticeshipListItemViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                ApprenticeName = apprenticeship.ApprenticeshipName,
                ApprenticeDateOfBirth = apprenticeship.DateOfBirth,
                TrainingCode = apprenticeship.TrainingCode,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                CanBeApproved = apprenticeship.CanBeApproved,
                OverlappingApprenticeships = overlappingApprenticeships.GetOverlappingApprenticeships(apprenticeship.Id)
            };
        }

        private async Task<ITrainingProgramme> GetTrainingProgramme(string trainingCode)
        {
            return (await GetTrainingProgrammes()).Where(x => x.Id == trainingCode).Single();
        }

        private async Task<List<ITrainingProgramme>> GetTrainingProgrammes()
        {
            var programmes = await _mediator.SendAsync(new GetTrainingProgrammesQueryRequest());

            return programmes.TrainingProgrammes;
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

        private static void AssertCommitmentStatus(Commitment commitment, params EditStatus[] allowedEditStatuses)
        {
            if (commitment == null)
                throw new InvalidStateException("Null commitment");

            if (!allowedEditStatuses.Contains(commitment.EditStatus))
                throw new InvalidStateException($"Invalid commitment state (edit status is {commitment.EditStatus}, expected {string.Join(",", allowedEditStatuses)})");
        }

        private Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors)
        {
            var dict = new Dictionary<string, string>();
            const string StartText = "The start date is not valid";
            const string EndText = "The end date is not valid";

            const string StartDateKey = "StartDateOverlap";
            const string EndDateKey = "EndDateOverlap";

            foreach (var item in overlappingErrors.GetFirstOverlappingApprenticeships())
            {
                switch (item.ValidationFailReason)
                {
                    case ValidationFailReason.OverlappingStartDate:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        break;
                    case ValidationFailReason.OverlappingEndDate:
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                    case ValidationFailReason.DateEmbrace:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                    case ValidationFailReason.DateWithin:
                        dict.AddIfNotExists(StartDateKey, StartText);
                        dict.AddIfNotExists(EndDateKey, EndText);
                        break;
                }
            }
            return dict;
        }
    }
}