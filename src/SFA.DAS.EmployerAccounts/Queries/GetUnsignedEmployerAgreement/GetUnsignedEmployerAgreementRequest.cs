using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement
{
    public class GetUnsignedEmployerAgreementRequest : IAsyncRequest<GetUnsignedEmployerAgreementResponse>
    {
        public string HashedAccountId { get; set; }
        public string ExternalUserId { get; set; }
    }
}