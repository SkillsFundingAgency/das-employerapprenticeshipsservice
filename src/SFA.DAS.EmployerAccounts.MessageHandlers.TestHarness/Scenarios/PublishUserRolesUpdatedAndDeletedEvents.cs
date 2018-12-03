using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios
{
    public class PublishUserRolesUpdatedAndDeletedEvents
    {
        private readonly IMessageSession _messageSession;

        public PublishUserRolesUpdatedAndDeletedEvents(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            Guid userRef = Guid.Parse("D543F6C7-87BF-42AD-B30A-2B27B9796B3F");
            long accountId = 2134;
            long userId = 5765876;
            var roles = new HashSet<UserRole> {UserRole.Viewer};

            var newUserEvent = new UserRolesUpdatedEvent(accountId, userRef, userId, roles, DateTime.Now) ;

            await _messageSession.Publish(newUserEvent);

            var updateUserEvent = new UserRolesUpdatedEvent(accountId, userRef, userId, roles, DateTime.Now);

            await _messageSession.Publish(updateUserEvent);

            var removeUserEvent = new UserRolesRemovedEvent(accountId, userId, DateTime.Now);

            await _messageSession.Publish(removeUserEvent);


        }
    }
}
