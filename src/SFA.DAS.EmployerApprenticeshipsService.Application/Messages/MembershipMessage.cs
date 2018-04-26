using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace SFA.DAS.EAS.Application.Messages
{
    public abstract class MembershipMessage : IAccountMessage, IUserMessage
    {
        [IgnoreMap]
        [Required]
        public long? AccountId { get; set; }

        [IgnoreMap]
        [Required]
        public string AccountHashedId { get; set; }

        [IgnoreMap]
        [Required]
        public long? UserId { get; set; }
    }
}