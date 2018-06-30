using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SFA.DAS.EAS.Infrastructure.Authorization;

namespace SFA.DAS.EAS.Application.Messages
{
    public class UserMessage : IUserMessage
    {
        [IgnoreMap]
        [Required]
        public Guid? UserRef { get; set; }
    }
}