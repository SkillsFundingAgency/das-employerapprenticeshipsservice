using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class AuthenticationOrchestraor
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AuthenticationOrchestraor(IMediator mediator, ILog logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            _logger = logger;
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