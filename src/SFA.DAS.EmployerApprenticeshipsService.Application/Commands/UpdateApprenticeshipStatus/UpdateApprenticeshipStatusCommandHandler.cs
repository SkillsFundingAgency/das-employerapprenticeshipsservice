using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Domain.Models.Apprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Application.Validation;
using System;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.EAS.Application.Commands.UpdateApprenticeshipStatus
{
    public sealed class UpdateApprenticeshipStatusCommandHandler : AsyncRequestHandler<UpdateApprenticeshipStatusCommand>
    {
        private IEmployerCommitmentApi _commitmentsApi;
        private readonly IValidator<UpdateApprenticeshipStatusCommand> _validator;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IMediator _mediator;

        public UpdateApprenticeshipStatusCommandHandler(IEmployerCommitmentApi commitmentsApi, IMediator mediator, ICurrentDateTime currentDateTime, IValidator<UpdateApprenticeshipStatusCommand> validator)
        {
            _commitmentsApi = commitmentsApi;
            _mediator = mediator;
            _currentDateTime = currentDateTime;
            _validator = validator;
        }

        protected override async Task HandleCore(UpdateApprenticeshipStatusCommand command)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var apprenticeshipSubmission = new ApprenticeshipSubmission
            {
                PaymentStatus = DeterminePaymentStatusForChange(command.ChangeType),
                DateOfChange = command.DateOfChange,
                UserId = command.UserId,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = command.UserEmailAddress, Name = command.UserDisplayName }
            };

            await ValidateDateOfChange(command, validationResult);

            await _commitmentsApi.PatchEmployerApprenticeship(command.EmployerAccountId, command.ApprenticeshipId, apprenticeshipSubmission);
        }

        private async Task ValidateDateOfChange(UpdateApprenticeshipStatusCommand command, Validation.ValidationResult validationResult)
        {
            if (command.ChangeType == ChangeStatusType.Stop) // Only need to validate date for stop currently
            {
                var response = await _mediator.SendAsync(new GetApprenticeshipQueryRequest { AccountId = command.EmployerAccountId, ApprenticeshipId = command.ApprenticeshipId });

                if (response.Apprenticeship.IsWaitingToStart(_currentDateTime))
                {
                    if (!command.DateOfChange.Equals(response.Apprenticeship.StartDate))
                    {
                        validationResult.AddError(nameof(command.DateOfChange), "Date must the same as start date if training hasn't started");
                        throw new InvalidRequestException(validationResult.ValidationDictionary);
                    }
                }
                else
                {
                    if (command.DateOfChange > _currentDateTime.Now.Date)
                    {
                        validationResult.AddError(nameof(command.DateOfChange), "Date must be a date in the past");
                        throw new InvalidRequestException(validationResult.ValidationDictionary);
                    }

                    if (response.Apprenticeship.StartDate > command.DateOfChange)
                    {
                        validationResult.AddError(nameof(command.DateOfChange), "Date cannot be earlier than training start date");
                        throw new InvalidRequestException(validationResult.ValidationDictionary);
                    }
                }
            }
        }

        private static PaymentStatus DeterminePaymentStatusForChange(ChangeStatusType changeType)
        {
            switch (changeType)
            {
                case ChangeStatusType.Pause:
                    return PaymentStatus.Paused;
                case ChangeStatusType.Resume:
                    return PaymentStatus.Active;
                case ChangeStatusType.Stop:
                    return PaymentStatus.Withdrawn;
                default:
                    throw new ArgumentOutOfRangeException(nameof(changeType), "Not a valid change type");
            }
        }
    }
}
