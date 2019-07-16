using SFA.DAS.EAS.Domain.Models.Transfers;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetAccountTransferTransactionDetails
{
    public class GetSenderTransferTransactionDetailsResponse
    {
        public string ReceiverAccountName { get; set; }
        public string ReceiverPublicHashedId { get; set; }
        public IEnumerable<AccountTransferDetails> TransferDetails { get; set; }
    }
}
