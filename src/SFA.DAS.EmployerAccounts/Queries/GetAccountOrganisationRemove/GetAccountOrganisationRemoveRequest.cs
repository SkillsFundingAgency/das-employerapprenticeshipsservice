using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountOrganisationRemove
{
    public class GetAccountOrganisationRemoveRequest : IAsyncRequest<GetAccountOrganisationRemoveResponse>
    {
        public string HashedAccountId { get; set; }
        public string UserId { get; set; }
        public string HashedAccountLegalEntityId { get; set; }
    }
}