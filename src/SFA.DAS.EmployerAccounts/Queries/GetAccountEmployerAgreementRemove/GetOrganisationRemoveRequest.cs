using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementRemove
{
    public class GetOrganisationRemoveRequest : IAsyncRequest<GetOrganisationRemoveResponse>
    {
        public string HashedAccountId { get; set; }
        public string UserId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
    }
}