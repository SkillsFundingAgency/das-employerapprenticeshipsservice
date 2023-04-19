namespace SFA.DAS.EmployerAccounts.Configuration
{
    /// <summary>
    /// Class to Read the GovUk SignIn Configuration.
    /// </summary>
    public class GovSignInIdentityConfiguration
    {
        /// <summary>
        /// Property to get or set the BaseUrl
        /// </summary>
        public string BaseUrl { get; set; }
        /// <summary>
        /// Property to get or set the User Registration link.
        /// </summary>
        public string RegisterLink { get; set; }
        /// <summary>
        /// Property to get or set the User SignIn Link.
        /// </summary>
        public string SignInLink { get; set; }
    }
}
