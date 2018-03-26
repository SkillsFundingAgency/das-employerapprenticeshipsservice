using SFA.DAS.EAS.Domain.Models.TransferRequests;

namespace SFA.DAS.EAS.Application.Dtos
{
    public class TransferRequestDto
    {
        public long CommitmentId { get; set; }
        public string CommitmentHashedId { get; set; }
        public AccountDto ReceiverAccount { get; set; }
        public AccountDto SenderAccount { get; set; }
        public TransferRequestStatus Status { get; set; }
        public decimal TransferCost { get; set; }
    }
}