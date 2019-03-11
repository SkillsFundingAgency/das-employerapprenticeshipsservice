using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole
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
                UserRole = (caller?.Role ?? Role.None)
            };
        }
    }
}
