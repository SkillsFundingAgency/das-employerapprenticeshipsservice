using SFA.DAS.EmployerFinance.Models.Transfers;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferTransactionDetails
{
    public class GetTransferTransactionDetailsResponse
    {
        public string SenderAccountName { get; set; }
        public string SenderAccountPublicHashedId { get; set; }
        public string ReceiverAccountName { get; set; }
        public string ReceiverAccountPublicHashedId { get; set; }
        public bool IsCurrentAccountSender { get; set; }
        public IEnumerable<AccountTransferDetails> TransferDetails { get; set; }
        public decimal TransferPaymentTotal { get; set; }
        public DateTime DateCreated { get; set; }
    }
}