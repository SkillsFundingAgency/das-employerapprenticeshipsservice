namespace SFA.DAS.EAS.Account.Api.Client
{
    public class AccountApiConfiguration : IAccountApiConfiguration
    {
        /// <summary>
        /// The base url (schema, server, port and application path as appropriate)
        /// </summary>
        /// <example>https://some-server/</example>
        public string ApiBaseUrl { get; set; }
        /// <summary>
        /// The clientId configured in Azure AD
        /// </summary>
        /// <example>97A08E90-925F-45C5-BDC9-AE2C249137A6</example>
        public string ClientId { get; set; }
        /// <summary>
        /// The code generated in Azure AD from your password - base 64 encoded
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// The location of the resource that you are trying to access in Azure AD
        /// </summary>
        public string IdentifierUri { get; set; }

        /// <summary>
        /// The tenant part of the authority url
        /// </summary>
        /// <example>xxxx.omicrosoft.com</example>
        public string Tenant { get; set; }
    }
}
