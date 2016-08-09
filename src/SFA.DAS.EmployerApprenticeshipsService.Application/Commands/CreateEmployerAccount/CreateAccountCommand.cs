using System;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount
{
    public class CreateAccountCommand : IAsyncRequest
    {
        public string ExternalUserId { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyRegisteredAddress { get; set; }
        public DateTime CompanyDateOfIncorporation { get; set; }
        public string EmployerRef { get; set; }
    }
}