using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class AuthenticationOrchestraor
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public AuthenticationOrchestraor(IMediator mediator, ILogger logger)
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