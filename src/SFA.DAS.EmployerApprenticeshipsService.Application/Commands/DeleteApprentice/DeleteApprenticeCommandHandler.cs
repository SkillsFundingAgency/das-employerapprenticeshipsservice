using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.DeleteApprentice
{
    public class DeleteApprenticeCommandHandler : AsyncRequestHandler<DeleteApprenticeCommand>
    {
        private readonly ICommitmentsService _commitmentsService;
        private readonly IValidator<DeleteApprenticeCommand> _validator;

        public DeleteApprenticeCommandHandler(ICommitmentsService commitmentsService, IValidator<DeleteApprenticeCommand> validator)
        {
            _validator = validator;
            _commitmentsService = commitmentsService;
        }

        protected override async Task HandleCore(DeleteApprenticeCommand message)
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
