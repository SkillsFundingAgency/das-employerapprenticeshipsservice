using System;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.EAS.Application.Commands.UpdateApprenticeship
{
    public class UpdateApprenticeshipCommandHandler : AsyncRequestHandler<UpdateApprenticeshipCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly UpdateApprenticeshipCommandValidator _validator;

        public UpdateApprenticeshipCommandHandler(IEmployerCommitmentApi commitmentsApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            _commitmentsApi = commitmentsApi;
            _validator = new UpdateApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(UpdateApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            await _commitmentsApi.UpdateEmployerApprenticeship(message.AccountId, message.Apprenticeship.CommitmentId, message.Apprenticeship.Id, 
                new ApprenticeshipRequest
                    {
                        Apprenticeship = message.Apprenticeship,
                        UserId = message.UserId
                    });
        }
    }
}