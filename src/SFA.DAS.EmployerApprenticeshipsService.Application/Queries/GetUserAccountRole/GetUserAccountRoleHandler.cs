﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetUserAccountRole
{
    public class GetUserAccountRoleHandler: IAsyncRequestHandler<GetUserAccountRoleQuery, GetUserAccountRoleResponse>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IValidator<GetUserAccountRoleQuery> _validator;

        public GetUserAccountRoleHandler(IValidator<GetUserAccountRoleQuery> validator, IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
            _validator = validator;
        }

        public async Task<GetUserAccountRoleResponse> Handle(GetUserAccountRoleQuery message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var caller = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            return new GetUserAccountRoleResponse
            {
                UserRole = (Role)(caller?.RoleId ?? (short)Role.None)
            };
        }
    }
}
