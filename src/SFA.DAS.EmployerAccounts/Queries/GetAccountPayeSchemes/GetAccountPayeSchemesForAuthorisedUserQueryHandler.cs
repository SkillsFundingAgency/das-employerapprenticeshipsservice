﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesForAuthorisedUserQueryHandler : IAsyncRequestHandler<GetAccountPayeSchemesForAuthorisedUserQuery, GetAccountPayeSchemesResponse>
    {
        private readonly IPayeSchemesService _payeSchemesService;
        private readonly IValidator<GetAccountPayeSchemesForAuthorisedUserQuery> _validator;

        public GetAccountPayeSchemesForAuthorisedUserQueryHandler(
            IPayeSchemesService payeSchemesService,
            IValidator<GetAccountPayeSchemesForAuthorisedUserQuery> validator)
        {
            _payeSchemesService = payeSchemesService;
            _validator = validator;
        }

        public async Task<GetAccountPayeSchemesResponse> Handle(GetAccountPayeSchemesForAuthorisedUserQuery message)
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

            var payeSchemes =
                (await _payeSchemesService
                    .GetPayeSchemsWithEnglishFractionForHashedAccountId(message.HashedAccountId))
                .ToList();

            return new GetAccountPayeSchemesResponse
            {
                PayeSchemes = payeSchemes
            };
        }
    }
}