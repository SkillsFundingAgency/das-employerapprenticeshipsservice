using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries
{
    internal class HasAgreementBeenSignedQuery : IReadStoreRequest<bool>
    {
        public long AccountId { get; }
        public int AgreementVersion { get; }
        public string AgreementType { get; }

        public HasAgreementBeenSignedQuery(long accountId, int agreementVersion, string agreementType)
        {
            AccountId = accountId;
            AgreementVersion = agreementVersion;
            AgreementType = agreementType;
        }
    }
}