using Newtonsoft.Json;

namespace SFA.DAS.EAS.Support.Infrastructure.Settings;

public class HmrcApiClientConfiguration : IHmrcApiClientConfiguration
{
    [JsonRequired]
    public string ApiBaseUrl { get; set; }
}
