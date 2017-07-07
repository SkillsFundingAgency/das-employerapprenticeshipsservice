using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetUser
{
    public class GetUserQueryHandler : IAsyncRequestHandler<GetUserQuery, GetUserResponse>
    {
        private readonly IUserRepository _repository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetUserQuery> _validator;

        public GetUserQueryHandler(IUserRepository repository, IHashingService hashingService,  IValidator<GetUserQuery> validator)
        {
            _repository = repository;
            _hashingService = hashingService;
            _validator = validator;
        }

        public async  Task<GetUserResponse> Handle(GetUserQuery message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            if (message.UserId < 1)
            {
                message.UserId = _hashingService.DecodeValue(message.HashedUserId);
            }

            var user = await _repository.GetUserById(message.UserId);

            return new GetUserResponse {User = user};
        }
    }
}
