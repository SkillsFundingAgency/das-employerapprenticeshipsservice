using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Types
{
    public class Agreement
    {
        [JsonProperty("hashedAgreementId")]
        public string HashedAgreementId { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("isPending")]
        public bool IsPending { get; set; }
    }
}