using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.MessageHandlers.TestHarness.Scenarios
{
    public class PublishCreateAccountUserEvents
    {
        private readonly IMessageSession _messageSession;

        public PublishCreateAccountUserEvents(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public async Task Run()
        {
            Guid userRef = Guid.Parse("A777F6C7-87BF-42AD-B30A-2B27B9796B3F");
            long accountId = 2134;
            var roles = new HashSet<UserRole> {UserRole.Viewer};

            var newUserEvent = new CreatedAccountEvent
            {
                AccountId = accountId,
                Name = "Test User",
                UserRef = userRef,
                Created = DateTime.Now
            };

            await _messageSession.Publish(newUserEvent);

            var updateUserEvent = new AccountUserRolesUpdatedEvent(accountId, userRef, roles, DateTime.Now);

            await _messageSession.Publish(updateUserEvent);

            var removeUserEvent = new AccountUserRemovedEvent(accountId, userRef, DateTime.Now);

            await _messageSession.Publish(removeUserEvent);
        }
    }
}
