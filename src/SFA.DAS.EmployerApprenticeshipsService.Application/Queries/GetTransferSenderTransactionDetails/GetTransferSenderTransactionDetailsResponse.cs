using SFA.DAS.EAS.Domain.Models.Transfers;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetTransferSenderTransactionDetails
{
    public class GetTransferSenderTransactionDetailsResponse
    {
        public DateTime DateCreated { get; set; }
        public string ReceiverAccountName { get; set; }
        public string ReceiverPublicHashedId { get; set; }
        public IEnumerable<AccountTransferDetails> TransferDetails { get; set; }
        public decimal TransferPaymentTotal { get; set; }
    }
}