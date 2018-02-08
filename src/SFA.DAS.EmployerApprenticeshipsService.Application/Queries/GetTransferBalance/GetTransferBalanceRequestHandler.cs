using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferBalance
{
    public class GetTransferBalanceRequestHandler : IAsyncRequestHandler<GetTransferBalanaceRequest, GetTransferBalanceResponse>
    {
        private readonly ITransferRepository _repository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetTransferBalanaceRequest> _validator;
        private readonly ILog _logger;

        public GetTransferBalanceRequestHandler(
            ITransferRepository repository,
            IHashingService hashingService,
            IValidator<GetTransferBalanaceRequest> validator,
            ILog logger)
        {
            _repository = repository;
            _hashingService = hashingService;
            _validator = validator;
            _logger = logger;
        }

        public async Task<GetTransferBalanceResponse> Handle(GetTransferBalanaceRequest message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            var balance = await _repository.GetTransferBalance(accountId);

            return new GetTransferBalanceResponse { Balance = balance };
        }
    }
}
