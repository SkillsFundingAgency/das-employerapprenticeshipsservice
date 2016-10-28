namespace SFA.DAS.EAS.Domain.Configuration
{
    public class IdentityServerConfiguration
    {
        public bool UseFake { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
    }
}