using SFA.DAS.EmployerFinance.Models.Account;

namespace SFA.DAS.EmployerFinance.Web.ViewModels
{
    public class FinanceDashboardViewModel
    {
        public decimal CurrentLevyFunds { get; set; }
        public string AccountHashedId { get; set; }
    }
}