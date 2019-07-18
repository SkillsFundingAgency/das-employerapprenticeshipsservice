using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Interfaces;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserAornLock
{
    public class GetUserAornLockQueryHandler : IAsyncRequestHandler<GetUserAornLockRequest, GetUserAornLockResponse>
    {
        private readonly IUserAornPayeLockService _userAornPayeLockService;

        public GetUserAornLockQueryHandler(IUserAornPayeLockService userAornPayeLockService)
        {
            _userAornPayeLockService = userAornPayeLockService;
        }

        public async Task<GetUserAornLockResponse> Handle(GetUserAornLockRequest message)
        {
            var status = await _userAornPayeLockService.UserAornPayeStatus(message.userRef);
            return new GetUserAornLockResponse { UserAornStatus = status };
        }
    }
}
