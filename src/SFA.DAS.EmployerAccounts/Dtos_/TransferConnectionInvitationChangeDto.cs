using System;
using SFA.DAS.EmployerAccounts.Models.TransferConnections;

namespace SFA.DAS.EmployerAccounts.Dtos
{
    public class TransferConnectionInvitationChangeDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public TransferConnectionInvitationStatus? Status { get; set; }
        public UserDto User { get; set; }
    }
}