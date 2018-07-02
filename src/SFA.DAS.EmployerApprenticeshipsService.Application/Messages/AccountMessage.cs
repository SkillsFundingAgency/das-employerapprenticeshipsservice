using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace SFA.DAS.EAS.Application.Messages
{
    public class AccountMessage : IAccountMessage
    {
        [IgnoreMap]
        [Required]
        public string AccountHashedId { get; set; }

        [IgnoreMap]
        [Required]
        public long? AccountId { get; set; }
    }
}