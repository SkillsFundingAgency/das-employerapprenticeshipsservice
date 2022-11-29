using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Queries.GetEnglishFractionCurrent
{
    public class GetEnglishFractionCurrentQueryHandler : IAsyncRequestHandler<GetEnglishFractionCurrentQuery, GetEnglishFractionCurrentResponse>
    {
        private readonly IValidator<GetEnglishFractionCurrentQuery> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IHashingService _hashingService;

        public GetEnglishFractionCurrentQueryHandler(IValidator<GetEnglishFractionCurrentQuery> validator, IDasLevyRepository dasLevyRepository, IHashingService hashingService)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
            _hashingService = hashingService;
        }

        public async Task<GetEnglishFractionCurrentResponse> Handle(GetEnglishFractionCurrentQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var fractions = await _dasLevyRepository.GetEnglishFractionCurrent(accountId, message.EmpRefs);

            return new GetEnglishFractionCurrentResponse { Fractions = fractions};
        }
    }
}
