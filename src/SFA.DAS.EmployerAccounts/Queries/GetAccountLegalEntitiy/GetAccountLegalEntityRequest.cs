using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiy
{
    public class GetAccountLegalEntityRequest : IAsyncRequest<GetAccountLegalEntityResponse>
    {
        public long AccountLegalEntityId { get; set; }
    }
}