using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Web.Models
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