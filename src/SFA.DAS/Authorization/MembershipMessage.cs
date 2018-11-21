using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SFA.DAS.Authorization.Mvc;

namespace SFA.DAS.Authorization
{
    public abstract class MembershipMessage : IAccountMessage, IUserMessage, IAccountViewModel
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