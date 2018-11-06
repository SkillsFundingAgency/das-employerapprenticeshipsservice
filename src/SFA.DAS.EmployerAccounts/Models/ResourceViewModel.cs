using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Models
{
    public class ResourceViewModel : IAccountResource
    {
        public string Id { get; set; }
        public string Href { get; set; }
    }
}