
namespace SFA.DAS.EAS.Account.Api.Types
{
    public class StatisticsViewModel
    {
        public long TotalAccounts { get; set; }
        public long TotalLegalEntities { get; set; }
        public long TotalPAYESchemes { get; set; }
        public long TotalAgreements { get; set; }
        public long TotalPayments { get; set; }

        public bool IsEmpty()
        {
            return TotalAccounts == 0 && TotalAgreements == 0 && TotalLegalEntities == 0 && TotalPAYESchemes == 0 &&
                   TotalPayments == 0;
        }
    }
}
