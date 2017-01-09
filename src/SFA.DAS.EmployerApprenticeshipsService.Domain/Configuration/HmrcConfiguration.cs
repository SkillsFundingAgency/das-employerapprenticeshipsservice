namespace SFA.DAS.EAS.Domain.Configuration
{
    public class HmrcConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public string ClientSecret { get; set; }
        public string ServerToken { get; set; }
        public string OgdSecret { get; set; }
        public string OgdClientId { get; set; }
    }
}