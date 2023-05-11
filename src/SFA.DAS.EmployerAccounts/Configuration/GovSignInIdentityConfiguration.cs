namespace SFA.DAS.EmployerAccounts.Configuration
{
    /// <summary>
    /// Class to Read the GovUk SignIn Configuration.
    /// </summary>
    public static class GovSignInIdentityConfiguration
    {
        /// <summary>
        /// Property to get the BaseUrl
        /// </summary>
        public const string BaseUrl = "https://www.gov.uk";

        /// <summary>
        /// Property to get the User Registration link.
        /// </summary>
        public const string RegisterLink = "/employing-an-apprentice/create-an-account";

        /// <summary>
        /// Property to get the User SignIn Link.
        /// </summary>
        public const string SignInLink = "/sign-in-apprenticeship-service-account";
    }
}
