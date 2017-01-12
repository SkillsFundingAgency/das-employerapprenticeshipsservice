using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Models.ReferenceData
{
    public class Charity
    {
        public int RegistrationNumber { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsRemoved { get; set; }
    }
}
