using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class UserAornLockOrchestrator
    {
        private readonly IMediator _mediator;      
        private readonly IMapper _mapper;
   
        protected UserAornLockOrchestrator()
        {
        }

        public UserAornLockOrchestrator(
            IMediator mediator, 
            IMapper mapper)
        {
            _mediator = mediator;                 
            _mapper = mapper;
        }

        public virtual async Task<OrchestratorResponse<UserAornLockStatusViewModel>> GetUserAornLockStatus(string userRef)
        {         
            var response = await _mediator.SendAsync(new GetUserAornLockRequest(userRef));

            return new OrchestratorResponse<UserAornLockStatusViewModel>
            {
                Data = _mapper.Map<UserAornLockStatusViewModel>(response.UserAornStatus)
            };
        }     
    }
}