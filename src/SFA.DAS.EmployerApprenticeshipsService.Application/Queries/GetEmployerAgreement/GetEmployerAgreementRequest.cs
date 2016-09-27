using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementRequest : IAsyncRequest<GetEmployerAgreementResponse>
    {
        public string HashedId { get; set; }
        public long AgreementId { get; set; }
        public string ExternalUserId { get; set; }
    }
}