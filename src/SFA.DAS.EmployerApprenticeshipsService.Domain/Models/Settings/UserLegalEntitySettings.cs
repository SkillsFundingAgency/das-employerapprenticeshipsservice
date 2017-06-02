using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Models.Settings
{
    public class UserLegalEntitySettings
    {
        public long EmployerAgreementId { get; set; }
        public long UserId { get; set; }
        public string LegalEntityName { get; set; }
        public bool ReceiveNotifications { get; set; }
    }
}
