using SFA.DAS.EAS.Domain.Models.Transfers;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels.Transactions
{
    public class TransferTransactionDetailsViewModel
    {
        public string SenderAccountName { get; set; }
        public string SenderAccountPublicHashedId { get; set; }
        public string ReceiverAccountName { get; set; }
        public string ReceiverAccountPublicHashedId { get; set; }
        public bool IsCurrentAccountSender { get; set; }
        public List<AccountTransferDetails> TransferDetails { get; set; }
        public decimal TransferPaymentTotal { get; set; }
        public DateTime DateCreated { get; set; }
    }
}