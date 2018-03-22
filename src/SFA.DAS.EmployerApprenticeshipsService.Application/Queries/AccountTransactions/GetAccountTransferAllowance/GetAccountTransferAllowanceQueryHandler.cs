using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransferAllowance
{
    public class GetAccountTransferAllowanceQueryHandler : IAsyncRequestHandler<GetAccountTransferAllowanceRequest, GetAccountTransferAllowanceResponse>
    {
        private readonly ITransferRepository _transferRepository;
        private readonly IValidator<GetAccountTransferAllowanceRequest> _validator;

        public GetAccountTransferAllowanceQueryHandler(ITransferRepository transferRepository, IValidator<GetAccountTransferAllowanceRequest> validator)
        {
            _transferRepository = transferRepository;
            _validator = validator;
        }

        public async Task<GetAccountTransferAllowanceResponse> Handle(GetAccountTransferAllowanceRequest message)
        {

            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var result = await _transferRepository.GetTransferAllowance(message.AccountId);

            return new GetAccountTransferAllowanceResponse {TransferAllowance = result};
        }
    }
}