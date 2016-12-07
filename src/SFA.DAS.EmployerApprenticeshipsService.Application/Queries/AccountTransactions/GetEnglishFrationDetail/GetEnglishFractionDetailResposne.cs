using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail
{
    public class GetEnglishFractionDetailResposne
    {
        public IEnumerable<DasEnglishFraction> FractionDetail { get; set; }
    }
}