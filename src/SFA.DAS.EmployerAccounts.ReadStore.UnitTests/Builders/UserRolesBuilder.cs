using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Builders
{
    internal class UserRolesBuilder
    {
        private readonly UserRoles _userRoles;

        public UserRolesBuilder()
        {
            _userRoles = (UserRoles)Activator.CreateInstance(typeof(UserRoles), true);
            _userRoles.SetPropertyTo(p => p.Roles, new HashSet<UserRole>());
        }

        public UserRolesBuilder WithId(Guid id)
        {
            _userRoles.SetPropertyTo(p => p.Id, id);

            return this;
        }

        public UserRolesBuilder WithAccountId(long accountId)
        {
            _userRoles.SetPropertyTo(p => p.AccountId, accountId);

            return this;
        }

        public UserRolesBuilder WithUserRef(Guid userRef)
        {
            _userRoles.SetPropertyTo(p => p.UserRef, userRef);

            return this;
        }

        public UserRolesBuilder WithRoles(HashSet<UserRole> roles)
        {
            _userRoles.SetPropertyTo(p => p.Roles, roles);

            return this;
        }

        public UserRolesBuilder WithUpdated(DateTime updated)
        {
            _userRoles.SetPropertyTo(p => p.Updated, updated);

            return this;
        }

        public UserRolesBuilder WithDeleted(DateTime? deleted)
        {
            _userRoles.SetPropertyTo(p => p.Deleted, deleted);

            return this;
        }

        public UserRolesBuilder WithOutboxMessage(OutboxMessage item)
        {
            var outboxData = (List<OutboxMessage>)_userRoles.OutboxData;

            outboxData.Clear();
            outboxData.Add(item);

            return this;
        }

        public UserRoles Build()
        {
            return _userRoles;
        }
    }
}