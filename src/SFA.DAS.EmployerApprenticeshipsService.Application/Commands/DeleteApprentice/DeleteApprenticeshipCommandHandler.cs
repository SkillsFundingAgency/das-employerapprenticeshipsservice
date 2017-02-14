using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Commands.DeleteApprentice
{
    public sealed class DeleteApprenticeshipCommandHandler : AsyncRequestHandler<DeleteApprenticeshipCommand>
    {
        private readonly ICommitmentsApi _commitmentsService;
        private readonly IValidator<DeleteApprenticeshipCommand> _validator;

        public DeleteApprenticeshipCommandHandler(
            ICommitmentsApi commitmentsApi, 
            IValidator<DeleteApprenticeshipCommand> validator
            )
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));

            _validator = validator;
            _commitmentsService = commitmentsApi;
        }

        protected override async Task HandleCore(DeleteApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _commitmentsService.DeleteEmployerApprenticeship(message.AccountId, message.ApprenticeshipId);
        }
    }
}
