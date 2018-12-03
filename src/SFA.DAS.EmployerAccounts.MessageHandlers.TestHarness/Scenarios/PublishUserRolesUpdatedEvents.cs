using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios
{
    public class PublishUserRolesUpdatedEvents
    {
        private readonly IMessageSession _messageSession;

        public PublishUserRolesUpdatedEvents(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            Guid userRef = Guid.NewGuid(); // Guid.Parse("3FCDF9A5-695B-4B16-A710-CBA708E2AA30");
            long accountId = 2134;
            long userId = 876876;
            var roles = new HashSet<UserRole> {UserRole.Viewer};

            var newUserEvent = new UserRolesUpdatedEvent(accountId, userRef, userId, roles, DateTime.Now) ;

            await _messageSession.Publish(newUserEvent);

            var updateUserEvent = new UserRolesUpdatedEvent(accountId, userRef, userId, roles, DateTime.Now);

            await _messageSession.Publish(updateUserEvent);
        }
    }
}
