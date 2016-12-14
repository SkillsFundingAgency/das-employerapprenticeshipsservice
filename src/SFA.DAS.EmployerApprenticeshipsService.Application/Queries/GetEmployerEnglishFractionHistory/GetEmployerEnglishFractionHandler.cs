using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeInUse;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory
{
    public class GetEmployerEnglishFractionHandler : IAsyncRequestHandler<GetEmployerEnglishFractionQuery, GetEmployerEnglishFractionResponse>
    {
        private readonly IValidator<GetEmployerEnglishFractionQuery> _validator;
        private readonly IDasLevyService _dasLevyService;
        private readonly IMediator _mediator;

        public GetEmployerEnglishFractionHandler(IValidator<GetEmployerEnglishFractionQuery> validator, IDasLevyService dasLevyService, IMediator mediator)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
            _mediator = mediator;
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

            var result = (await _dasLevyService.GetEnglishFractionHistory(message.EmpRef)).ToList();

            var schemeInformation = await _mediator.SendAsync(new GetPayeSchemeInUseQuery {Empref = message.EmpRef});

            return new GetEmployerEnglishFractionResponse {Fractions = result, EmpRef = message.EmpRef,EmpRefAddedDate = schemeInformation.PayeScheme.AddedDate};
        }
    }
}
