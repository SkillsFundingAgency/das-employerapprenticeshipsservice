using MediatR;

namespace SFA.DAS.EmployerAccounts.Queries.GetLegalEntity
{
    public class GetLegalEntityQuery : IAsyncRequest<GetLegalEntityResponse>
    {
        public GetLegalEntityQuery(string accountHashedId, long legalEntityId, bool includeAllAgreements)
        {
            AccountHashedId = accountHashedId;
            LegalEntityId = legalEntityId;
            IncludeAllAgreements = includeAllAgreements;
        }

        public string AccountHashedId { get; }

        public long LegalEntityId { get; }

        public bool IncludeAllAgreements { get; }
    }
}