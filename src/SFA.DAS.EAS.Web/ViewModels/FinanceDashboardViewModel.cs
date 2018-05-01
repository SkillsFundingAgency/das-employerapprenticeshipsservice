using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class FinanceDashboardViewModel
    {
        public decimal CurrentLevyFunds { get; set; }
        public Account Account { get; set; }
    }
}