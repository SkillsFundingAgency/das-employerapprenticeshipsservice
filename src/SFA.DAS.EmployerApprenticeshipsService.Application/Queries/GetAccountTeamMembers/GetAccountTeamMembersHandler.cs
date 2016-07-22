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
            var validationResult = _validator.Validate(message);
            if (validationResult.IsValid())
            {
                var accounts = await _repository.GetAccountTeamMembersForUserId(message.Id, message.UserId);
                return new GetAccountTeamMembersResponse {TeamMembers = accounts};
            }
            return new GetAccountTeamMembersResponse();
        }
    }
}