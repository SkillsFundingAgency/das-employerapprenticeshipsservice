using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class AuthenticationOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public AuthenticationOrchestrator(IMediator mediator, ILog logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            _logger = logger;
        }

        public async Task SaveIdentityAttributes(Guid externalUserId, string email, string firstName, string lastName)
        {
            await _mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                EmailAddress = email,
                ExternalUserId = externalUserId,
                LastName = lastName,
                FirstName = firstName
            });
        }
    }
}