using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.Levy;

namespace SFA.DAS.EmployerAccounts.Queries.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailResposne
    {
        public IEnumerable<DasEnglishFraction> FractionDetail { get; set; }
    }
}