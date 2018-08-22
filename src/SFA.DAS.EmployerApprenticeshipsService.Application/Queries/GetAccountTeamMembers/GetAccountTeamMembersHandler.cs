﻿using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTeamMembers
{
    public class GetAccountTeamMembersHandler : IAsyncRequestHandler<GetAccountTeamMembersQuery, GetAccountTeamMembersResponse>
    {
        private readonly IValidator<GetAccountTeamMembersQuery> _validator;
        private readonly IAccountTeamRepository _repository;

        public GetAccountTeamMembersHandler(IValidator<GetAccountTeamMembersQuery> validator, IAccountTeamRepository repository)
        {
            _validator = validator;
            _repository = repository;
        }

        public async Task<GetAccountTeamMembersResponse> Handle(GetAccountTeamMembersQuery message)
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

            var accounts = await _repository.GetAccountTeamMembersForUserId(message.HashedAccountId, message.ExternalUserId);
            return new GetAccountTeamMembersResponse {TeamMembers = accounts};
            
        }
    }
}