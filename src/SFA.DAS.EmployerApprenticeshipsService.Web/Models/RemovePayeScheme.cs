namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class RemovePayeScheme :ViewModelBase
    {
        public string UserId { get; set; }
        public long AccountId { get; set; }
        public string PayeRef { get; set; }
        public string AccountName { get; set; }
        public bool RemoveScheme { get; set; }

        public string RemoveSchemeErrorMessage => GetErrorMessage(nameof(RemoveScheme));
    }
}