using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    public class SignAccountAgreementCommand : IReadStoreRequest<Unit>
    {
        public SignAccountAgreementCommand(long accountId, int agreementVersion, string agreementType)
        {
            AccountId = accountId;
            AgreementVersion = agreementVersion;
            AgreementType = agreementType;
        }

        public long AccountId { get; set; }
        public int AgreementVersion { get; set; }
        public string AgreementType { get; set; }
    }
}
