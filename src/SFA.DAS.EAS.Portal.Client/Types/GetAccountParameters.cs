namespace SFA.DAS.EAS.Portal.Client.Types
{
    public class GetAccountParameters
    {
        /// <summary>
        /// The (non-public) hashed account id.
        /// </summary>
        public string HashedAccountId { get; set; }
        
        /// <summary>
        /// The maximum number of vacancies to return.
        /// </summary>
        public int MaxNumberOfVacancies { get; set; }
    }
}