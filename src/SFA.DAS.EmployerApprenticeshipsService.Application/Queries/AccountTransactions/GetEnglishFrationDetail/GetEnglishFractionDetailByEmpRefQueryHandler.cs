using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailByEmpRefQueryHandler : IAsyncRequestHandler<GetEnglishFractionDetailByEmpRefQuery, GetEnglishFractionDetailResposne>
    {
        private readonly IValidator<GetEnglishFractionDetailByEmpRefQuery> _validator;
        private readonly IDasLevyRepository _dasLevyRepository;

        public GetEnglishFractionDetailByEmpRefQueryHandler(IValidator<GetEnglishFractionDetailByEmpRefQuery> validator, IDasLevyRepository dasLevyRepository)
        {
            _validator = validator;
            _dasLevyRepository = dasLevyRepository;
        }

        public async Task<GetEnglishFractionDetailResposne> Handle(GetEnglishFractionDetailByEmpRefQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var fractionDetail = await _dasLevyRepository.GetEnglishFractionHistory(message.EmpRef);

            return new GetEnglishFractionDetailResposne {FractionDetail = fractionDetail};
        }
    }
}
