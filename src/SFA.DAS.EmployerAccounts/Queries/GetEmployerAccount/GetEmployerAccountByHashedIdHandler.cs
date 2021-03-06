﻿using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount
{
    public class GetEmployerAccountByHashedIdHandler : IAsyncRequestHandler<GetEmployerAccountByHashedIdQuery, GetEmployerAccountByHashedIdResponse>
    {
        private readonly IEmployerAccountRepository _employerAccountRepository;
        private readonly IValidator<GetEmployerAccountByHashedIdQuery> _validator;

        public GetEmployerAccountByHashedIdHandler(
            IEmployerAccountRepository employerAccountRepository,
            IValidator<GetEmployerAccountByHashedIdQuery> validator)
        {
            _employerAccountRepository = employerAccountRepository;
            _validator = validator;
        }

        public async Task<GetEmployerAccountByHashedIdResponse> Handle(GetEmployerAccountByHashedIdQuery message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            if (result.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var employerAccount = await _employerAccountRepository.GetAccountByHashedId(message.HashedAccountId);

            return new GetEmployerAccountByHashedIdResponse
            {
                Account = employerAccount
            };
        }
    }
}