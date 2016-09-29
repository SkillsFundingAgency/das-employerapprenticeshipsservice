using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountTeamMembers
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

            var accounts = await _repository.GetAccountTeamMembersForUserId(message.HashedId, message.ExternalUserId);
            return new GetAccountTeamMembersResponse {TeamMembers = accounts};
            
        }
    }
}