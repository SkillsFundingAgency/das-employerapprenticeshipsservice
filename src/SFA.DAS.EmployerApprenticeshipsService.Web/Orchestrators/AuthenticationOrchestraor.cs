using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.UpsertRegisteredUser;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
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

        internal void SaveIdentityAttributes(string userRef, string email, string firstName, string lastName)
        {
            _mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                EmailAddress = email,
                UserRef = userRef,
                LastName = lastName,
                FirstName = firstName
            });
        }
    }
}