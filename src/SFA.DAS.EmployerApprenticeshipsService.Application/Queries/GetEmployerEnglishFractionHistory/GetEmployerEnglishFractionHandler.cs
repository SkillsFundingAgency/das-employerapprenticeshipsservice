using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory
{
    public class GetEmployerEnglishFractionHandler : IAsyncRequestHandler<GetEmployerEnglishFractionQuery, GetEmployerEnglishFractionResponse>
    {
        private readonly IValidator<GetEmployerEnglishFractionQuery> _validator;
        private readonly IDasLevyService _dasLevyService;

        public GetEmployerEnglishFractionHandler(IValidator<GetEmployerEnglishFractionQuery> validator, IDasLevyService dasLevyService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
        }

        public async Task<GetEmployerEnglishFractionResponse> Handle(GetEmployerEnglishFractionQuery message)
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

            var result = await _dasLevyService.GetEnglishFractionHistory(message.EmpRef);

            return new GetEmployerEnglishFractionResponse {Fractions = result};
        }
    }
}
