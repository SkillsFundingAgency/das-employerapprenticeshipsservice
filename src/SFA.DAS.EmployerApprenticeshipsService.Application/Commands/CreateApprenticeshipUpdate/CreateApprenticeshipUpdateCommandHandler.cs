using System;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.CreateApprenticeshipUpdate
{
    public class CreateApprenticeshipUpdateCommandHandler : AsyncRequestHandler<CreateApprenticeshipUpdateCommand>
    {
        private readonly IEmployerCommitmentApi _commitmentsApi;
        private readonly IValidator<CreateApprenticeshipUpdateCommand> _validator;

        public CreateApprenticeshipUpdateCommandHandler(
            IEmployerCommitmentApi commitmentsApi, 
            IValidator<CreateApprenticeshipUpdateCommand> validator)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));

            _commitmentsApi = commitmentsApi;
            _validator = validator;
        }

        protected override async Task HandleCore(CreateApprenticeshipUpdateCommand command)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var request = new ApprenticeshipUpdateRequest
            {
                ApprenticeshipUpdate = command.ApprenticeshipUpdate,
                UserId = command.UserId
            };

            await _commitmentsApi.CreateApprenticeshipUpdate(command.EmployerId, request);
        }
    }
}