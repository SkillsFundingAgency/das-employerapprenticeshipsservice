using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class AddNewPayeScheme
    {
        public string PayeScheme { get; set; }
        public List<LegalEntity> LegalEntities { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string HashedId { get; set; }
    }
}