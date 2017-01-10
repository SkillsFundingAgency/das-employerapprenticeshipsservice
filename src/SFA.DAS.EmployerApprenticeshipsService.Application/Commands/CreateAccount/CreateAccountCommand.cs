using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.CreateAccount
{
    public class CreateAccountCommand : IAsyncRequest<CreateAccountCommandResponse>
    {
        public string ExternalUserId { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyRegisteredAddress { get; set; }
        public DateTime CompanyDateOfIncorporation { get; set; }
        public string EmployerRef { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string CompanyStatus { get; set; }
        public string EmployerRefName { get; set; }
    }
}