using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SFA.DAS.EAS.Domain;

namespace SFA.DAS.EAS.Application.Messages
{
    public abstract class AuthorizedMessage : IAuthorizedMessage
    {
        [IgnoreMap]
        [Required]
        public Guid? UserExternalId { get; set; }

        [IgnoreMap]
        [Required]
        [RegularExpression(Constants.AccountHashedIdRegex)]
        public string AccountHashedId { get; set; }
    }
}