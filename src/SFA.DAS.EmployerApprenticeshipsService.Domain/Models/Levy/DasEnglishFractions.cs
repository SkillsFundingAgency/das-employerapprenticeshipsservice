using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy
{
    public class DasEnglishFractions
    {
        public string  Id { get; set; }
        public DateTime DateCalculated { get; set; }
        public decimal Amount { get; set; }
    }
}
