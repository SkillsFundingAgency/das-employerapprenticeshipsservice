using SFA.DAS.EAS.Domain.Models.Transfers;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels.Transactions
{
    public class TransferSenderTransactionDetailsViewModel
    {
        public DateTime DateCreated { get; set; }
        public string ReceiverAccountName { get; set; }
        public string ReceiverAccountPublicHashedId { get; set; }
        public List<AccountTransferDetails> TransferDetails { get; set; }
        public decimal TransferPaymentTotal { get; set; }
    }
}