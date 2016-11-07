using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail
{
    public class GetEmployerAccountTransactionDetailHandler : IAsyncRequestHandler<GetEmployerAccountTransactionDetailQuery, GetEmployerAccountTransactionDetailResponse>
    {
        private readonly IValidator<GetEmployerAccountTransactionDetailQuery> _validator;
        private readonly IDasLevyService _dasLevyService;

        public GetEmployerAccountTransactionDetailHandler(IValidator<GetEmployerAccountTransactionDetailQuery> validator, IDasLevyService dasLevyService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
        }

        public async Task<GetEmployerAccountTransactionDetailResponse> Handle(GetEmployerAccountTransactionDetailQuery message)
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

            var data = await _dasLevyService.GetTransactionDetailById(message.Id);

            return new GetEmployerAccountTransactionDetailResponse {TransactionDetail = data};

        }
    }
}