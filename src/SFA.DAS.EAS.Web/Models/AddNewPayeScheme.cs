using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Web.Models
{
    public class AddNewPayeScheme
    {
        public string PayeScheme { get; set; }
        public List<LegalEntity> LegalEntities { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string HashedAccountId { get; set; }
        public bool EmprefNotFound { get; set; }
        public string PayeName { get; set; }
    }
}