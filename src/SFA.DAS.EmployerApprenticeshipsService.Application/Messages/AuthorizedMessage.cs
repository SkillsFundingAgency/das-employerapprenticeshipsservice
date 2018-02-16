using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace SFA.DAS.EAS.Application.Messages
{
    public abstract class AuthorizedMessage : IAuthorizedMessage
    {
        [IgnoreMap]
        [Required]
        public string AccountHashedId { get; set; }

        [IgnoreMap]
        [Required]
        public long? AccountId { get; set; }

        [IgnoreMap]
        [Required]
        public Guid? UserExternalId { get; set; }

        [IgnoreMap]
        [Required]
        public long? UserId { get; set; }
    }
}