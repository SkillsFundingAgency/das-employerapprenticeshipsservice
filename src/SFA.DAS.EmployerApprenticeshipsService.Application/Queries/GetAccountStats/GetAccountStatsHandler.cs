using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetAccountStats
{
    public class GetAccountStatsHandler : IAsyncRequestHandler<GetAccountStatsQuery, GetAccountStatsResponse>
    {
        private readonly IAccountRepository _repository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetAccountStatsQuery> _validator;

        public GetAccountStatsHandler(IAccountRepository repository, IHashingService hashingService, IValidator<GetAccountStatsQuery> validator)
        {
            _repository = repository;
            _hashingService = hashingService;
            _validator = validator;
        }

        public async Task<GetAccountStatsResponse> Handle(GetAccountStatsQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                if (validationResult.IsUnauthorized)
                {
                    throw new UnauthorizedAccessException("User not authorised");
                }

                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var accoundId = _hashingService.DecodeValue(message.HashedAccountId);

            var stats = await _repository.GetAccountStats(accoundId);

            return new GetAccountStatsResponse { Stats = stats };
        }
    }
}
