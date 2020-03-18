using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementType
{
    public class GetEmployerAgreementTypeRequest : IAsyncRequest<GetEmployerAgreementTypeResponse>
    {
        public string HashedAgreementId { get; set; }
    }
}