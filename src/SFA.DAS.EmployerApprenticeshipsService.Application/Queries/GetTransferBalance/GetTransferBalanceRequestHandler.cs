using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.Queries.GetTransferBalance
{
    public class GetTransferBalanceRequestHandler : IAsyncRequestHandler<GetTransferBalanaceRequest, GetTransferBalanceResponse>
    {
        private readonly ITransferRepository _repository;
        private readonly IValidator<GetTransferBalanaceRequest> _validator;
        private readonly ILog _logger;

        public GetTransferBalanceRequestHandler(
            ITransferRepository repository,
            IValidator<GetTransferBalanaceRequest> validator,
            ILog logger)
        {
            _repository = repository;
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

            decimal? balance = null;

            try
            {
                balance = await _repository.GetTransferBalance(message.HashedAccountId);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Could not get transfer balance from repository for account Id {message.HashedAccountId}");
            }

            return new GetTransferBalanceResponse { Balance = balance };
        }
    }
}
