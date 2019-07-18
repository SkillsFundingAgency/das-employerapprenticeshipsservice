using System;
using SFA.DAS.EAS.Domain.Models.TransferConnections;

namespace SFA.DAS.EAS.Application.Dtos
{
    public class TransferConnectionInvitationChangeDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public TransferConnectionInvitationStatus? Status { get; set; }
        public UserDto User { get; set; }
    }
}