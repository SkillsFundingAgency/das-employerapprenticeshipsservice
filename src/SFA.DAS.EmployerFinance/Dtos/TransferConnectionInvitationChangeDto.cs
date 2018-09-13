using System;
using SFA.DAS.EmployerFinance.Models.TransferConnections;

namespace SFA.DAS.EmployerFinance.Dtos
{
    public class TransferConnectionInvitationChangeDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public TransferConnectionInvitationStatus? Status { get; set; }
        public UserDto User { get; set; }
    }
}