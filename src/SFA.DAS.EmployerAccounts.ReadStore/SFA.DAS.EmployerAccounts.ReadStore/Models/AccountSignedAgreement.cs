using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models
{
    public class AccountSignedAgreement : Document
    {
        [JsonProperty("accountId")]
        public long AccountId { get; private set; }

        [JsonProperty("agreementVersion")]
        public int AgreementVersion { get; private set; }

        [JsonProperty("agreementType")]
        public string AgreementType { get; private set; }

        public AccountSignedAgreement(long accountId, int agreementVersion, string agreementType)
        {
            AccountId = accountId;
            AgreementVersion = agreementVersion;
            AgreementType = agreementType;
        }

        [JsonConstructor]
        private AccountSignedAgreement()
        {
        }

        public bool IsSigned(string agreementType, int minimumAgreementVersion)
        {
            return agreementType == AgreementType && AgreementVersion >= minimumAgreementVersion;
        }
    }
}
