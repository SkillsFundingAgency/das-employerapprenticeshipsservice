using System.ComponentModel.DataAnnotations;
using SFA.DAS.EAS.Domain.Models.TransferConnection;

namespace SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionQuery
{
    public class SentTransferConnectionViewModel
    {
        [Required]
        [Range(1, 2)]
        public int Choice { get; set; }

        public Domain.Data.Entities.Account.Account ReceiverAccount { get; set; }
        public TransferConnection TransferConnection { get; set; }
    }
}