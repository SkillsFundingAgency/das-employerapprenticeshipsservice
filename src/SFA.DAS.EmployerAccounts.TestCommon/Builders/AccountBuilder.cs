﻿using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;

namespace SFA.DAS.EmployerAccounts.TestCommon.Builders
{
    public class AccountBuilder
    {
        private readonly Account _account = new Account();

        public AccountBuilder WithId(long id)
        {
            _account.Id = id;

            return this;
        }

        public Account Build()
        {
            return _account;
        }

        public AccountBuilder WithReceivedTransferConnectionInvitation(
            TransferConnectionInvitation transferConnectionInvitation)
        {
            _account.ReceivedTransferConnectionInvitations.Add(transferConnectionInvitation);

            return this;
        }

        public AccountBuilder WithSentTransferConnectionInvitation(
            TransferConnectionInvitation transferConnectionInvitation)
        {
            _account.SentTransferConnectionInvitations.Add(transferConnectionInvitation);

            return this;
        }
    }
}