using MediatR;
using SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class AuthenticationOrchestrator
    {
        private readonly IMediator _mediator;

        public AuthenticationOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SaveIdentityAttributes(string userRef, string email, string firstName, string lastName)
        {
            await _mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                EmailAddress = email,
                UserRef = userRef,
                LastName = lastName,
                FirstName = firstName
            });
        }
    }
}