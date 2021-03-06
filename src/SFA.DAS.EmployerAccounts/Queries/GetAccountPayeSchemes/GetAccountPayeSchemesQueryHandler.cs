﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes
{
    public class GetAccountPayeSchemesQueryHandler : IAsyncRequestHandler<GetAccountPayeSchemesQuery, GetAccountPayeSchemesResponse>
    {
        private readonly IValidator<GetAccountPayeSchemesQuery> _validator;
        private IPayeSchemesService _payeSchemesService;

        public GetAccountPayeSchemesQueryHandler(
            IPayeSchemesService payeSchemesService,
            IValidator<GetAccountPayeSchemesQuery> validator )
        {
            _validator = validator;
            _payeSchemesService = payeSchemesService;
        }

        public async Task<GetAccountPayeSchemesResponse> Handle(GetAccountPayeSchemesQuery message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
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