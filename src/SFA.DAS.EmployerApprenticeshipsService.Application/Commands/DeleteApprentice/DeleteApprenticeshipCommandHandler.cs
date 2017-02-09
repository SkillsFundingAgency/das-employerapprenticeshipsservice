using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.DeleteApprentice
{
    public sealed class DeleteApprenticeshipCommandHandler : AsyncRequestHandler<DeleteApprenticeshipCommand>
    {
        private readonly ICommitmentsService _commitmentsService;
        private readonly IValidator<DeleteApprenticeshipCommand> _validator;

        public DeleteApprenticeshipCommandHandler(ICommitmentsService commitmentsService, IValidator<DeleteApprenticeshipCommand> validator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (commitmentsService == null)
                throw new ArgumentNullException(nameof(commitmentsService));

            _validator = validator;
            _commitmentsService = commitmentsService;
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
