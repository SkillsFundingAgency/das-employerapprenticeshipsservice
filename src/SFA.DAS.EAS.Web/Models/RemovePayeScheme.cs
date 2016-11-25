namespace SFA.DAS.EAS.Web.Models
{
    public class RemovePayeScheme :ViewModelBase
    {
        public string UserId { get; set; }
        public string HashedId { get; set; }
        public string PayeRef { get; set; }
        public string AccountName { get; set; }
        public int RemoveScheme { get; set; }

        public string RemoveSchemeErrorMessage => GetErrorMessage(nameof(RemoveScheme));
    }
}