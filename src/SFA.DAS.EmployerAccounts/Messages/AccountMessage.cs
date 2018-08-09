using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Messages
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