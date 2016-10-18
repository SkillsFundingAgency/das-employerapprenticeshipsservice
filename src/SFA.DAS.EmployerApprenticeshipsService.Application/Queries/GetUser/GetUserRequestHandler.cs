using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUser
{
    public class GetUserRequestHandler : IAsyncRequestHandler<GetUserRequest, GetUserResponse>
    {
        private readonly IUserRepository _repository;

        public GetUserRequestHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async  Task<GetUserResponse> Handle(GetUserRequest message)
        {
            var user = await _repository.GetUserById(message.UserId);

            return new GetUserResponse {User = user};
        }
    }
}
