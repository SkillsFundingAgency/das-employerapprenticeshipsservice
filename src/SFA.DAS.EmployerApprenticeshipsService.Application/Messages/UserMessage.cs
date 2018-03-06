using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace SFA.DAS.EAS.Application.Messages
{
    public class UserMessage
    {
        [IgnoreMap]
        [Required]
        public long? UserId { get; set; }
    }
}