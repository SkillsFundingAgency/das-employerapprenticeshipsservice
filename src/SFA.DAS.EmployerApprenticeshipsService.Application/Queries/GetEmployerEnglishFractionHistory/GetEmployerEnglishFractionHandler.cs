using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeInUse;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory
{
    public class GetEmployerEnglishFractionHandler : IAsyncRequestHandler<GetEmployerEnglishFractionQuery, GetEmployerEnglishFractionResponse>
    {
        private readonly IValidator<GetEmployerEnglishFractionQuery> _validator;
        private readonly IDasLevyService _dasLevyService;
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;

        public GetEmployerEnglishFractionHandler(IValidator<GetEmployerEnglishFractionQuery> validator, IDasLevyService dasLevyService, IMediator mediator, IHashingService hashingService)
        {
            _validator = validator;
            _dasLevyService = dasLevyService;
            _mediator = mediator;
            _hashingService = hashingService;
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

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);
            var result = (await _dasLevyService.GetEnglishFractionHistory(accountId, message.EmpRef)).ToList();

            var schemeInformation = await _mediator.SendAsync(new GetPayeSchemeInUseQuery {Empref = message.EmpRef});

            return new GetEmployerEnglishFractionResponse {Fractions = result, EmpRef = message.EmpRef,EmpRefAddedDate = schemeInformation.PayeScheme.AddedDate};
        }
    }
}
