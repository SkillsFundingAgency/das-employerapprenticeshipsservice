using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.Queries.GetUsers
{
    public class GetUsersQueryHandler : IAsyncRequestHandler<GetUsersQuery, GetUsersQueryResponse>
    {
        private readonly IUserAccountRepository _userRepository;

        public GetUsersQueryHandler(IUserAccountRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUsersQueryResponse> Handle(GetUsersQuery message)
        {
            var users = await _userRepository.GetAllUsers();

            return new GetUsersQueryResponse
            {
                Users = users.UserList
            };
        }
    }
}
