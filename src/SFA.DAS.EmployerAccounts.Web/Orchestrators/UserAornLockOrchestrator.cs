using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;
using SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class UserAornLockOrchestrator
    {
        private readonly IMediator _mediator;      
       
        protected UserAornLockOrchestrator()
        {
        }

        public UserAornLockOrchestrator(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        public virtual async Task<OrchestratorResponse<UserAornPayeStatus>> GetUserAornLockStatus(string userRef)
        {         
            var response = await _mediator.SendAsync(new GetUserAornLockRequest(userRef));

            return new OrchestratorResponse<UserAornPayeStatus>
            {
                Data = response.UserAornStatus
            };
        }

        public virtual async Task UpdateUserAornLockStatus(string userRef, bool success)
        {
            await _mediator.SendAsync(new UpdateUserAornLockRequest(userRef, success));
        }
    }
}