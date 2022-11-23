using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion
{
    public class GetMinimumSignedAgreementVersionQuery : IAsyncRequest<GetMinimumSignedAgreementVersionResponse>
    {
        public long AccountId { get; set; }
    }
}