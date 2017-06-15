using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Queries.GetTeamMembers
{
    public class GetTeamMembersRequestHandler : IAsyncRequestHandler<GetTeamMembersRequest, GetTeamMembersResponse>
    {
        private readonly IAccountTeamRepository _repository;
        private readonly IValidator<GetTeamMembersRequest> _validator;
        private readonly ILog _logger;

        public GetTeamMembersRequestHandler(
            IAccountTeamRepository repository, 
            IValidator<GetTeamMembersRequest> validator,
            ILog logger)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<GetTeamMembersResponse> Handle(GetTeamMembersRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            _logger.Info($"Getting team members for account id {message.HashedAccountId}");

            var teamMembers = await _repository.GetAccountTeamMembers(message.HashedAccountId);

            return new GetTeamMembersResponse
            {
                TeamMembers = teamMembers
            };
        }
    }
}
