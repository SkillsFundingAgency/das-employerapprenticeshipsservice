﻿using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.Commitments.Api.Types.Validation.Types;
using SFA.DAS.EAS.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public class ApprenticeshipMapper : IApprenticeshipMapper
    {
        private readonly IHashingService _hashingService;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IMediator _mediator;

        public ApprenticeshipMapper(
            IHashingService hashingService,
            ICurrentDateTime currentDateTime,
            IMediator mediator)
        {
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (currentDateTime == null)
                throw new ArgumentNullException(nameof(currentDateTime));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            _hashingService = hashingService;
            _currentDateTime = currentDateTime;
            _mediator = mediator;
        }

        public ApprenticeshipDetailsViewModel MapToApprenticeshipDetailsViewModel(Apprenticeship apprenticeship)
        {
            var pendingChange = PendingChanges.None;
            if (apprenticeship.PendingUpdateOriginator == Originator.Employer)
                pendingChange = PendingChanges.WaitingForApproval;
            if (apprenticeship.PendingUpdateOriginator == Originator.Provider)
                pendingChange = PendingChanges.ReadyForApproval;
            
            var statusText = MapPaymentStatus(apprenticeship.PaymentStatus, apprenticeship.StartDate);

            return new ApprenticeshipDetailsViewModel
            {
                HashedApprenticeshipId = _hashingService.HashValue(apprenticeship.Id),
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                DateOfBirth = apprenticeship.DateOfBirth,
                StartDate = apprenticeship.StartDate,
                EndDate = apprenticeship.EndDate,
                TrainingName = apprenticeship.TrainingName,
                Cost = apprenticeship.Cost,
                Status = statusText,
                ProviderName = apprenticeship.ProviderName,
                PendingChanges = pendingChange,
                Alert = MapRecordStatus(apprenticeship.PendingUpdateOriginator, apprenticeship.DataLockTriageStatus),
                EmployerReference = apprenticeship.EmployerRef,
                CohortReference = _hashingService.HashValue(apprenticeship.CommitmentId),
                EnableEdit = pendingChange == PendingChanges.None
                            && new []{ PaymentStatus.Active, PaymentStatus.Paused,  }.Contains(apprenticeship.PaymentStatus),
                CanEditStatus = !(new List<PaymentStatus> { PaymentStatus.Completed, PaymentStatus.Withdrawn }).Contains(apprenticeship.PaymentStatus),
                HasDataLockError = apprenticeship.DataLockTriageStatus != null 
                                && apprenticeship.DataLockTriageStatus == TriageStatus.Restart,
                DataLockTriageStatus = apprenticeship.DataLockTriageStatus ?? TriageStatus.Unknown
            };
        }
        
        public ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship)
        {
            var isStartDateInFuture = apprenticeship.StartDate.HasValue && apprenticeship.StartDate.Value >
                                      new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

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
                TrainingCode = apprenticeship.TrainingCode,
                TrainingName = apprenticeship.TrainingName,
                Cost = NullableDecimalToString(apprenticeship.Cost),
                StartDate = new DateTimeViewModel(apprenticeship.StartDate),
                EndDate = new DateTimeViewModel(apprenticeship.EndDate),
                PaymentStatus = apprenticeship.PaymentStatus,
                AgreementStatus = apprenticeship.AgreementStatus,
                ProviderRef = apprenticeship.ProviderRef,
                EmployerRef = apprenticeship.EmployerRef,
                HasStarted = !isStartDateInFuture
            };
        }

        public async Task<Apprenticeship> MapFromAsync(ApprenticeshipViewModel viewModel)
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

            if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode))
            {
                var training = await GetTrainingProgramme(viewModel.TrainingCode);
                apprenticeship.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                apprenticeship.TrainingCode = viewModel.TrainingCode;
                apprenticeship.TrainingName = training.Title;
            }

            return apprenticeship;
        }

        public async Task<Apprenticeship> MapFrom(ApprenticeshipViewModel viewModel)
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
                Cost = viewModel.Cost.AsNullableDecimal(),
                StartDate = viewModel.StartDate.DateTime,
                EndDate = viewModel.EndDate.DateTime,
                ProviderRef = viewModel.ProviderRef,
                EmployerRef = viewModel.EmployerRef
            };

            if (!string.IsNullOrWhiteSpace(viewModel.TrainingCode))
            {
                var training = await GetTrainingProgramme(viewModel.TrainingCode);
                apprenticeship.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                apprenticeship.TrainingCode = viewModel.TrainingCode;
                apprenticeship.TrainingName = training.Title;
            }

            return apprenticeship;
        }
        

        public Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors)
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

        public ApprenticeshipUpdate MapFrom(UpdateApprenticeshipViewModel viewModel)
        {
            var apprenticeshipId = _hashingService.DecodeValue(viewModel.HashedApprenticeshipId);
            return new ApprenticeshipUpdate
            {
                ApprenticeshipId =  apprenticeshipId,
                Cost = viewModel.Cost.AsNullableDecimal(),
                DateOfBirth = viewModel.DateOfBirth?.DateTime,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                StartDate = viewModel.StartDate?.DateTime,
                EndDate = viewModel.EndDate?.DateTime,
                Originator = Originator.Employer,
                Status = ApprenticeshipUpdateStatus.Pending,
                TrainingName = viewModel.TrainingName, 
                TrainingCode = viewModel.TrainingCode,
                TrainingType = viewModel.TrainingType,
                EmployerRef = viewModel.EmployerRef
            };
        }

        public UpdateApprenticeshipViewModel MapFrom(ApprenticeshipUpdate apprenticeshipUpdate)
        {
            return new UpdateApprenticeshipViewModel
            {
                Cost = NullableDecimalToString(apprenticeshipUpdate.Cost),
                DateOfBirth = new DateTimeViewModel(apprenticeshipUpdate.DateOfBirth),
                FirstName = apprenticeshipUpdate.FirstName,
                LastName = apprenticeshipUpdate.LastName,
                StartDate = new DateTimeViewModel(apprenticeshipUpdate.StartDate),
                EndDate = new DateTimeViewModel(apprenticeshipUpdate.EndDate),
                TrainingName = apprenticeshipUpdate.TrainingName,
                TrainingCode = apprenticeshipUpdate.TrainingCode,
                TrainingType = apprenticeshipUpdate.TrainingType,
                EmployerRef = apprenticeshipUpdate.EmployerRef
            };
        }

        public async Task<UpdateApprenticeshipViewModel> CompareAndMapToApprenticeshipViewModel(
            Apprenticeship original, ApprenticeshipViewModel edited)
        {
            Func<string, string, string> changedOrNull = (a, edit) => 
                a?.Trim() == edit?.Trim() ? null : edit;

            var apprenticeshipDetailsViewModel = MapToApprenticeshipDetailsViewModel(original);
            var model = new UpdateApprenticeshipViewModel
            {
                FirstName = changedOrNull(original.FirstName, edited.FirstName),
                LastName = changedOrNull(original.LastName, edited.LastName),
                DateOfBirth = original.DateOfBirth == edited.DateOfBirth.DateTime
                    ? null
                    : edited.DateOfBirth,
                Cost = original.Cost == edited.Cost.AsNullableDecimal() ? null : edited.Cost,
                StartDate =  original.StartDate == edited.StartDate.DateTime
                  ? null
                  : edited.StartDate,
                EndDate = original.EndDate == edited.EndDate.DateTime 
                    ? null
                    : edited.EndDate,
                EmployerRef =  original.EmployerRef?.Trim() == edited.EmployerRef?.Trim()
                            || (string.IsNullOrEmpty(original.EmployerRef)  && string.IsNullOrEmpty(edited.EmployerRef))
                    ? null 
                    : edited.EmployerRef ?? "",
                OriginalApprenticeship = apprenticeshipDetailsViewModel
            };

            if (!string.IsNullOrWhiteSpace(edited.TrainingCode) && original.TrainingCode != edited.TrainingCode)
            {
                var training = await GetTrainingProgramme(edited.TrainingCode);
                model.TrainingType = training is Standard ? TrainingType.Standard : TrainingType.Framework;
                model.TrainingCode = edited.TrainingCode;
                model.TrainingName = training.Title;
            }

            return model;
        }

        private async Task<ITrainingProgramme> GetTrainingProgramme(string trainingCode)
        {
            // TODO: LWA - Need to check is this is called multiple times in a single request.
            var trainingProgrammes = await _mediator.SendAsync(new GetTrainingProgrammesQueryRequest());

            return trainingProgrammes.TrainingProgrammes.Where(x => x.Id == trainingCode).Single();
        }

        private static string NullableDecimalToString(decimal? item)
        {
            return (item.HasValue) ? ((int)item).ToString() : string.Empty;
        }

        private string MapPaymentStatus(PaymentStatus paymentStatus, DateTime? apprenticeshipStartDate)
        {
            var now = new DateTime(_currentDateTime.Now.Year, _currentDateTime.Now.Month, 1);
            var waitingToStart = apprenticeshipStartDate.HasValue && apprenticeshipStartDate.Value > now;

            switch (paymentStatus)
            {
                case PaymentStatus.PendingApproval:
                    return "Approval needed";
                case PaymentStatus.Active:
                    return waitingToStart ? "Waiting to start" : "Live";
                case PaymentStatus.Paused:
                    return "Paused";
                case PaymentStatus.Withdrawn:
                    return "Stopped";
                case PaymentStatus.Completed:
                    return "Finished";
                case PaymentStatus.Deleted:
                    return "Deleted";
                default:
                    return string.Empty;
            }
        }

        private string MapRecordStatus(Originator? pendingUpdateOriginator, TriageStatus? dataLockTriageStatus)
        {
            var changeText = string.Empty;
            var dataLockText = string.Empty;

            if (pendingUpdateOriginator != null)
            {
                changeText = pendingUpdateOriginator == Originator.Employer
                    ? "Changes pending" 
                    : "Changes for review" ;
            }

            if (dataLockTriageStatus != null)
            {
                switch (dataLockTriageStatus)
                {
                    case TriageStatus.Restart:
                        dataLockText = "Changes requested";
                        break;
                }
            }

            return !string.IsNullOrEmpty(dataLockText) ? dataLockText : changeText;
        }
    }
}