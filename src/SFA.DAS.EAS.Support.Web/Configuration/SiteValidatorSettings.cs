using Newtonsoft.Json;

namespace SFA.DAS.EAS.Support.Web.Configuration
{
    public interface ISiteValidatorSettings
    {
        string Tenant { get; }

        string Audience { get; }

        string Scope { get; }
    }

    public class SiteValidatorSettings : ISiteValidatorSettings
    {
        public string Tenant { get; set; }

        [JsonRequired]
        public string Audience { get; set; }

        [JsonRequired]
        public string Scope { get; set; }
    }
}
