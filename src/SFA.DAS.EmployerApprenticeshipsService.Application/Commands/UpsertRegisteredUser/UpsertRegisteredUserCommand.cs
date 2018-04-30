using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommand : IAsyncRequest
    {
        public Guid ExternalUserId{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; } 
    }
}
