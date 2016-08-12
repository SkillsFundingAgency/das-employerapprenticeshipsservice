using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class Scheme
    {
        public int Id { get; set; }
        public string Ref { get; set; }
        public long AccountId { get; set; }
    }
}
