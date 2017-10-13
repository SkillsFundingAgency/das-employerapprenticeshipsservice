using System;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.EAS.Application.Commands.ReviewApprenticeshipUpdate
{
    public sealed class ReviewApprenticeshipUpdateCommandHandler : AsyncRequestHandler<ReviewApprenticeshipUpdateCommand>
    {
        private readonly Validation.IValidator<ReviewApprenticeshipUpdateCommand> _validator;

        private readonly IEmployerCommitmentApi _commitmentsApi;

        public ReviewApprenticeshipUpdateCommandHandler(Validation.IValidator<ReviewApprenticeshipUpdateCommand> validator, 
            IEmployerCommitmentApi commitmentsApi)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _validator = validator;
            _commitmentsApi = commitmentsApi;

        }
        protected override async Task HandleCore(ReviewApprenticeshipUpdateCommand command)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid())
                throw new ValidationException("Validation failed");

            await _commitmentsApi.PatchApprenticeshipUpdate(command.AccountId, command.ApprenticeshipId,
                new ApprenticeshipUpdateSubmission
                    {
                        UpdateStatus = command.IsApproved ? ApprenticeshipUpdateStatus.Approved : ApprenticeshipUpdateStatus.Rejected,
                        UserId = command.UserId,
                        LastUpdatedByInfo = new LastUpdateInfo {  EmailAddress = command.UserEmailAddress, Name = command.UserDisplayName }
                    });
        }
    }
}
