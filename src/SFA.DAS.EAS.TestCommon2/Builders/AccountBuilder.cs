using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.TestCommon.Builders
{
    public class AccountBuilder
    {
        private readonly Domain.Models.Account.Account _account = new Domain.Models.Account.Account();

        public AccountBuilder WithId(long id)
        {
            _account.Id = id;

            return this;
        }

        public Domain.Models.Account.Account Build()
        {
            return _account;
        }

        public AccountBuilder WithReceivedTransferConnectionInvitation(TransferConnectionInvitation transferConnectionInvitation)
        {
            _account.ReceivedTransferConnectionInvitations.Add(transferConnectionInvitation);

            return this;
        }

        public AccountBuilder WithSentTransferConnectionInvitation(TransferConnectionInvitation transferConnectionInvitation)
        {
            _account.SentTransferConnectionInvitations.Add(transferConnectionInvitation);

            return this;
        }
    }
}