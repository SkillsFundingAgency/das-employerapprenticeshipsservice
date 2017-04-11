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
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using System.Collections.Generic;
using FluentValidation;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Application.Commands.CreateApprenticeshipUpdate;
using SFA.DAS.EAS.Application.Commands.ReviewApprenticeshipUpdate;
using SFA.DAS.EAS.Application.Commands.UndoApprenticeshipUpdate;
using SFA.DAS.EAS.Application.Queries.GetApprenticeshipUpdate;
using SFA.DAS.EAS.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EAS.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.EAS.Web.Exceptions;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public sealed class EmployerManageApprenticeshipsOrchestrator : CommitmentsBaseOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly IApprenticeshipMapper _apprenticeshipMapper;
        private readonly ILogger _logger;
        private readonly ICookieStorageService<ApprenticeshipViewModel> _apprenticshipsViewModelCookieStorageService;

        private const string CookieName = "sfa-das-employerapprenticeshipsservice-apprentices";

        public EmployerManageApprenticeshipsOrchestrator(IMediator mediator, IHashingService hashingService, IApprenticeshipMapper apprenticeshipMapper, ILogger logger, ICookieStorageService<ApprenticeshipViewModel> apprenticshipsViewModelCookieStorageService) : base(mediator, hashingService, logger)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _apprenticeshipMapper = apprenticeshipMapper;
            _logger = logger;
            _apprenticshipsViewModelCookieStorageService = apprenticshipsViewModelCookieStorageService;
        }

        public async Task<OrchestratorResponse<ManageApprenticeshipsViewModel>> GetApprenticeships(string hashedAccountId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            _logger.Info($"Getting On-programme apprenticeships for empployer: {accountId}");

            return await CheckUserAuthorization(async () =>
            {
                    var data = await _mediator.SendAsync(new GetAllApprenticeshipsRequest { AccountId = accountId });

                    var apprenticeships = 
                        data.Apprenticeships
                        .OrderBy(m => m.ApprenticeshipName)
                        .Select(m => _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(m, default(ApprenticeshipUpdate)))
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
                    var data = await _mediator.SendAsync(
                        new GetApprenticeshipQueryRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId });

                    var updateReponse = await _mediator.SendAsync(
                        new GetApprenticeshipUpdateRequest { AccountId = accountId, ApprenticeshipId = apprenticeshipId } );

                    var detailsViewModel = 
                        _apprenticeshipMapper.MapToApprenticeshipDetailsViewModel(data.Apprenticeship, updateReponse.ApprenticeshipUpdate);

                    return new OrchestratorResponse<ApprenticeshipDetailsViewModel> { Data = detailsViewModel };
                }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<ExtendedApprenticeshipViewModel>> GetApprenticeshipForEdit(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Info($"Getting Approved Apprenticeship for Editing, Account: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
            {
                await AssertApprenticeshipStatus(accountId, apprenticeshipId);
                // TODO: LWA Assert that the apprenticeship can be edited - Story says should be allowed to go to edit details page??

                var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId
                });

                AssertApprenticeshipIsEditable(data.Apprenticeship);
                var apprenticeship = _apprenticeshipMapper.MapToApprenticeshipViewModel(data.Apprenticeship);

                apprenticeship.HashedAccountId = hashedAccountId;

                return new OrchestratorResponse<ExtendedApprenticeshipViewModel>
                {
                    Data = new ExtendedApprenticeshipViewModel
                    {
                        Apprenticeship = apprenticeship,
                        ApprenticeshipProgrammes = await GetTrainingProgrammes(),
                    }
                };
            }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<UpdateApprenticeshipViewModel>> GetConfirmChangesModel(string hashedAccountId, string hashedApprenticeshipId, string externalUserId, ApprenticeshipViewModel apprenticeship)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Debug($"Getting confirm change model: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(async () =>
                {
                    await AssertApprenticeshipStatus(accountId, apprenticeshipId);

                    var data = await _mediator.SendAsync(new GetApprenticeshipQueryRequest
                    {
                        AccountId = accountId,  
                        ApprenticeshipId = apprenticeshipId
                    });

                    var apprenticeships = _apprenticeshipMapper.CompareAndMapToApprenticeshipViewModel(data.Apprenticeship, apprenticeship);

                    return new OrchestratorResponse<UpdateApprenticeshipViewModel>
                               {
                                   Data = await apprenticeships
                    };
                }, hashedAccountId, externalUserId);
        }

        public async Task<OrchestratorResponse<UpdateApprenticeshipViewModel>> GetViewChangesViewModel(string hashedAccountId, string hashedApprenticeshipId, string externalUserId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Debug($"Getting confirm change model: {accountId}, ApprenticeshipId: {apprenticeshipId}");

            return await CheckUserAuthorization(
                async () =>
                    {
                        var data = await _mediator.SendAsync(
                            new GetApprenticeshipUpdateRequest
                            {
                                AccountId = accountId,
                                ApprenticeshipId = apprenticeshipId
                            });

                        var apprenticeship = await _mediator.SendAsync(
                            new GetApprenticeshipQueryRequest {
                                AccountId = accountId,
                                ApprenticeshipId = apprenticeshipId
                            });

                        var viewModel = _apprenticeshipMapper.MapFrom(data.ApprenticeshipUpdate);
                        viewModel.OriginalApprenticeship = apprenticeship.Apprenticeship;
                        viewModel.HashedAccountId = hashedAccountId;
                        viewModel.HashedApprenticeshipId = hashedApprenticeshipId;

                        viewModel.ProviderName = apprenticeship.Apprenticeship.ProviderName;

                        return new OrchestratorResponse<UpdateApprenticeshipViewModel>
                                 {
                                     Data = viewModel 
                                 };
                    }, hashedAccountId, externalUserId);
        }

        public async Task SubmitUndoApprenticeshipUpdate(string hashedAccountId, string hashedApprenticeshipId, string userId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            _logger.Debug($"Undoing pending update for : AccountId {accountId}, ApprenticeshipId: {apprenticeshipId}");

            await CheckUserAuthorization(async () =>
            {
                await _mediator.SendAsync(new UndoApprenticeshipUpdateCommand
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId,
                    UserId = userId
                });
            }
            , hashedAccountId, userId);
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

        private async Task<List<ITrainingProgramme>> GetTrainingProgrammes()
        {
            var programmes = await _mediator.SendAsync(new GetTrainingProgrammesQueryRequest());

            return programmes.TrainingProgrammes;
        }

        public async Task CreateApprenticeshipUpdate(UpdateApprenticeshipViewModel apprenticeship, string hashedAccountId, string userId)
        {
            var employerId = _hashingService.DecodeValue(hashedAccountId);
            await _mediator.SendAsync(new CreateApprenticeshipUpdateCommand
                {
                    EmployerId = employerId,
                    ApprenticeshipUpdate = _apprenticeshipMapper.MapFrom(apprenticeship),
                    UserId = userId
                });
        }

        private async Task AssertApprenticeshipStatus(long accountId, long apprenticeshipId)
        {
            var result = await _mediator.SendAsync(new GetApprenticeshipUpdateRequest
                                    {
                                        AccountId = accountId,
                                        ApprenticeshipId = apprenticeshipId
                                    });

            if(result.ApprenticeshipUpdate != null)
                throw new InvalidStateException("Pending apprenticeship update");
        }

        public ApprenticeshipViewModel GetApprenticeshipViewModelFromCookie()
        {
            var model = _apprenticshipsViewModelCookieStorageService.Get(CookieName);

            return model;
        }

        public UpdateApprenticeshipViewModel GetUpdateApprenticeshipViewModelFromCookie()
        {
            var model = _apprenticshipsViewModelCookieStorageService.Get(CookieName);

            return (UpdateApprenticeshipViewModel) model;
        }


        public async Task<OrchestratorResponse<UpdateApprenticeshipViewModel>> GetOrchestratorResponseUpdateApprenticeshipViewModelFromCookie(string hashedAccountId, string hashedApprenticeshipId)
        {
            var model = _apprenticshipsViewModelCookieStorageService.Get(CookieName);

            var mappedModel = await _apprenticeshipMapper.MapToUpdateApprenticeshipViewModel(model);

            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var apprenticeship = await _mediator.SendAsync(
                            new GetApprenticeshipQueryRequest
                            {
                                AccountId = accountId,
                                ApprenticeshipId = apprenticeshipId
                            });

            mappedModel.OriginalApprenticeship = apprenticeship.Apprenticeship;
            mappedModel.HashedAccountId = hashedAccountId;
            mappedModel.HashedApprenticeshipId = hashedApprenticeshipId;

            return new OrchestratorResponse<UpdateApprenticeshipViewModel> {Data = mappedModel };
        }

        public void CreateApprenticeshipViewModelCookie(ApprenticeshipViewModel model)
        {
            _apprenticshipsViewModelCookieStorageService.Delete(CookieName);
            _apprenticshipsViewModelCookieStorageService.Create(model,CookieName);
        }
        public async Task SubmitReviewApprenticeshipUpdate(string hashedAccountId, string hashedApprenticeshipId, string userId, bool isApproved)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var apprenticeshipId = _hashingService.DecodeValue(hashedApprenticeshipId);

            await CheckUserAuthorization(async () =>
            {
                await _mediator.SendAsync(new ReviewApprenticeshipUpdateCommand
                {
                    AccountId = accountId,
                    ApprenticeshipId = apprenticeshipId,
                    UserId = userId,
                    IsApproved = isApproved
                });
            }
            ,hashedAccountId, userId);
        }

        private void AssertApprenticeshipIsEditable(Apprenticeship apprenticeship)
        {
            var isStartDateInFuture = apprenticeship.StartDate.HasValue && apprenticeship.StartDate.Value >
                                      new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var editable = isStartDateInFuture
                         && apprenticeship.PaymentStatus == PaymentStatus.Active;

            if (!editable)
            {
                throw new ValidationException("Unable to edit apprenticeship - not waiting to start");
            }
        }
    }
}

