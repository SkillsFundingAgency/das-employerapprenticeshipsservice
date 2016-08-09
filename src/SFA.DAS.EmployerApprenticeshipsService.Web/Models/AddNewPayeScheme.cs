using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class AddNewPayeScheme
    {
        public string PayeScheme { get; set; }
        public long AccountId { get; set; }
        public LegalEntities LegalEntities { get; set; }
    }
}