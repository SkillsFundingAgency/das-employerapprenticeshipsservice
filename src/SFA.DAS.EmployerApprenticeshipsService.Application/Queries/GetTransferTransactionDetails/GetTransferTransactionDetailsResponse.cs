using SFA.DAS.EAS.Domain.Models.Transfers;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Application.Queries.GetTransferTransactionDetails
{
    public class GetTransferTransactionDetailsResponse
    {
        public string SenderAccountName { get; set; }
        public string SenderPublicHashedId { get; set; }
        public string ReceiverAccountName { get; set; }
        public string ReceiverPublicHashedId { get; set; }
        public bool IsCurrentAccountSender { get; set; }
        public IEnumerable<AccountTransferDetails> TransferDetails { get; set; }
        public decimal TransferPaymentTotal { get; set; }
        public DateTime DateCreated { get; set; }
    }
}