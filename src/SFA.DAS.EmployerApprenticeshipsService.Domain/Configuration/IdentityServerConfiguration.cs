namespace SFA.DAS.EAS.Domain.Configuration
{
    public class IdentityServerConfiguration
    {
        public bool UseFake { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string AuthorizeEndPoint { get; set; }
        public string LogoutEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string UserInfoEndpoint { get; set; }
        public string ClaimsBaseUrl { get; set; }
    }
}