using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public abstract class UserVerificationOrchestratorBase
    {
        public IMediator Mediator { get; set; }

        protected UserVerificationOrchestratorBase()
        {

        }

        protected UserVerificationOrchestratorBase(IMediator mediator)
        {
            Mediator = mediator;
        }

        public virtual async Task<GetUserAccountRoleResponse> GetUserAccountRole(string hashedAccountId, string externalUserId)
        {
            return await Mediator.SendAsync(new GetUserAccountRoleQuery
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId
            });
        }
    }
}