using System;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.EAS.Application.Commands.UndoApprenticeshipUpdate
{
    public sealed class UndoApprenticeshipUpdateCommandHandler : AsyncRequestHandler<UndoApprenticeshipUpdateCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly Validation.IValidator<UndoApprenticeshipUpdateCommand> _validator;

        public UndoApprenticeshipUpdateCommandHandler(
            Validation.IValidator<UndoApprenticeshipUpdateCommand> validator, 
            IEmployerCommitmentApi commitmentsApi)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _validator = validator;
            _commitmentsApi = commitmentsApi;
        }

        protected override async Task HandleCore(UndoApprenticeshipUpdateCommand command)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid())
            {
                throw new ValidationException(validationResult.ValidationDictionary.FirstOrDefault().Value);
            }

            var submission = new ApprenticeshipUpdateSubmission
                                 {
                                     UpdateStatus = ApprenticeshipUpdateStatus.Deleted,
                                     UserId = command.UserId,
                                     LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = command.UserEmailAddress, Name = command.UserDisplayName }
                                 };

            await _commitmentsApi.PatchApprenticeshipUpdate(command.AccountId, command.ApprenticeshipId, submission);
        }
    }
}