using System;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount
{
    public class CreateAccountCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public string CompanyNumber { get; set; }
        public string CompanyName { get; set; }
        public string EmployerRef { get; set; }
    }
}