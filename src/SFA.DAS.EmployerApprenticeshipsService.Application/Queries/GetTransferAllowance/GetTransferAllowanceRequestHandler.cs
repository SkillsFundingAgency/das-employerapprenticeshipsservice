using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceRequestHandler : IAsyncRequestHandler<GetTransferAllowanceRequest, GetTransferAllowanceResponse>
    {
        private readonly ITransferRepository _repository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetTransferAllowanceRequest> _validator;
        private readonly ILog _logger;

        public GetTransferAllowanceRequestHandler(
            ITransferRepository repository,
            IHashingService hashingService,
            IValidator<GetTransferAllowanceRequest> validator,
            ILog logger)
        {
            _repository = repository;
            _hashingService = hashingService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<GetTransferAllowanceResponse> Handle(GetTransferAllowanceRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            _logger.Debug($"Getting transfer allowance for account ID {message.HashedAccountId}");

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            var balance = await _repository.GetTransferBalance(accountId);

            _logger.Debug($"Retrieved transfer allowance of {balance} for account ID {message.HashedAccountId}");

            return new GetTransferAllowanceResponse { Balance = balance };
        }
    }
}
