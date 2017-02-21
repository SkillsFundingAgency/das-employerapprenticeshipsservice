using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Models.EmployerAgreement
{
    public class SignEmployerAgreement
    {
        public long AgreementId { get; set; }
        public long SignedById { get; set; }
        public string SignedByName { get; set; }
        public DateTime SignedDate { get; set; }
    }
}
