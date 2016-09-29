using System;
using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommand : IAsyncRequest
    {
        public long AgreementId { get; set; }
        public string HashedId { get; set; }
        public string ExternalUserId { get; set; }
        public DateTime SignedDate { get; set; }
    }
}