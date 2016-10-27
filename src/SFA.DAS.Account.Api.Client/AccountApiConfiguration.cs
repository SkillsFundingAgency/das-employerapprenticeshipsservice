namespace SFA.DAS.Account.Api.Client
{
    public class AccountApiConfiguration
    {
        /// <summary>
        /// The JWT token issued to your service for access to the API
        /// </summary>
        public string ClientToken { get; set; }

        /// <summary>
        /// The base url (schema, server, port and application path as appropriate)
        /// </summary>
        /// <example>https://some-server/</example>
        public string ApiBaseUrl { get; set; }
    }
}
