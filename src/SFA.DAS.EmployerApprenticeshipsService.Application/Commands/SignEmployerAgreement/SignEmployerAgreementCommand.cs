using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommand : IAsyncRequest
    {
        public long AgreementId { get; set; }
        public long AccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}