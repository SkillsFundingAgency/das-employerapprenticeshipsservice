using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Queries.GetEnglishFrationHistory
{
    public class GetEnglishFractionHistoryResposne
    {
        public IEnumerable<DasEnglishFraction> FractionDetail { get; set; }
    }
}