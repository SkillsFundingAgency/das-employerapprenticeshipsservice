using SFA.DAS.EAS.Domain.Models.Transfers;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetTransferSenderTransactionDetails
{
    public class GetTransferSenderTransactionDetailsResponse
    {
        public string ReceiverAccountName { get; set; }
        public string ReceiverPublicHashedId { get; set; }
        public IEnumerable<AccountTransferDetails> TransferDetails { get; set; }
    }
}