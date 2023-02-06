using System.Threading.Tasks;
using SFA.DAS.Authorization.Results;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.EmployerAccounts.Api.Authorization
{
    public class LocalAuthorisationService : IAuthorizationService
    {
        public void Authorize(params string[] options)
        {
            
        }

        public Task AuthorizeAsync(params string[] options)
        {
            return Task.CompletedTask;
        }

        public AuthorizationResult GetAuthorizationResult(params string[] options)
        {
            return new AuthorizationResult();
        }

        public Task<AuthorizationResult> GetAuthorizationResultAsync(params string[] options)
        {
            return Task.FromResult(new AuthorizationResult());
        }

        public bool IsAuthorized(params string[] options)
        {
            return true;
        }

        public Task<bool> IsAuthorizedAsync(params string[] options)
        {
           return Task.FromResult<bool>(true);
        }
    }
}
