namespace SFA.DAS.EAS.Account.Api.Client
{
    public interface IAccountApiConfiguration
    {
        /// <summary>
        /// The base url (schema, server, port and application path as appropriate)
        /// </summary>
        /// <example>https://some-server/</example>
        string ApiBaseUrl { get; }

        /// <summary>
        /// The clientId configured in Azure AD
        /// </summary>
        /// <example>97A08E90-925F-45C5-BDC9-AE2C249137A6</example>
        string ClientId { get; }

        /// <summary>
        /// The code generated in Azure AD from your password - base 64 encoded
        /// </summary>
        string ClientSecret { get; }

        /// <summary>
        /// The location of the resource that you are trying to access in Azure AD
        /// </summary>
        string IdentifierUri { get; }

        /// <summary>
        /// The tenant part of the authority url
        /// </summary>
        /// <example>xxxx.omicrosoft.com</example>
        string Tenant { get; }
    }
}