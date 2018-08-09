using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Messages
{
    public abstract class MembershipMessage : IAccountMessage, IUserMessage
    {
        [IgnoreMap]
        [Required]
        public string AccountHashedId { get; set; }

        [IgnoreMap]
        [Required]
        public long? AccountId { get; set; }

        [IgnoreMap]
        [Required]
        public Guid? UserRef { get; set; }
    }
}