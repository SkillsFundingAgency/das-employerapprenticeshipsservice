using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetUser
{
    public class GetUserQueryHandler : IAsyncRequestHandler<GetUserQuery, GetUserResponse>
    {
        private readonly IUserRepository _repository;
        private readonly IValidator<GetUserQuery> _validator;

        public GetUserQueryHandler(IUserRepository repository, IValidator<GetUserQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async  Task<GetUserResponse> Handle(GetUserQuery message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var user = await _repository.GetUserById(message.UserId);

            return new GetUserResponse {User = user};
        }
    }
}
