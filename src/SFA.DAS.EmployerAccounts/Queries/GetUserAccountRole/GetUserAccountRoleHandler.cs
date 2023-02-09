using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;

public class GetUserAccountRoleHandler: IRequestHandler<GetUserAccountRoleQuery, GetUserAccountRoleResponse>
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IValidator<GetUserAccountRoleQuery> _validator;

    public GetUserAccountRoleHandler(IValidator<GetUserAccountRoleQuery> validator, IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
        _validator = validator;
    }

    public async Task<GetUserAccountRoleResponse> Handle(GetUserAccountRoleQuery message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        var caller = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

        return new GetUserAccountRoleResponse
        {
            UserRole = (caller?.Role ?? Role.None)
        };
    }
}