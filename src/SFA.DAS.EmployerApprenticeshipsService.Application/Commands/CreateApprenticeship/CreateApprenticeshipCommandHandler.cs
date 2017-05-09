using System;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.EAS.Application.Commands.CreateApprenticeship
{
    public class CreateApprenticeshipCommandHandler : AsyncRequestHandler<CreateApprenticeshipCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly CreateApprenticeshipCommandValidator _validator;

        public CreateApprenticeshipCommandHandler(IEmployerCommitmentApi commitmentsApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            _commitmentsApi = commitmentsApi;
            _validator = new CreateApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(CreateApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var apprenticeshipRequest = new ApprenticeshipRequest
            {
                Apprenticeship = message.Apprenticeship,
                UserId = message.UserId,
                LastUpdatedByInfo = new LastUpdateInfo { EmailAddress = message.UserEmailAddress, Name = message.UserDisplayName }
            };
            await _commitmentsApi.CreateEmployerApprenticeship(message.AccountId, message.Apprenticeship.CommitmentId, apprenticeshipRequest);
        }
    }
}