
namespace SFA.DAS.EAS.Account.Api.Types
{
    public class StatisticsViewModel
    {
        public long TotalAccounts { get; set; }
        public long TotalActiveLegalEntities { get; set; }
        public long TotalPAYESchemes { get; set; }
        public long TotalSignedAgreements { get; set; }
        public long TotalPaymentsThisYear { get; set; }

        public bool IsEmpty()
        {
            return TotalAccounts == 0 && TotalSignedAgreements == 0 && TotalActiveLegalEntities == 0 && TotalPAYESchemes == 0 &&
                   TotalPaymentsThisYear == 0;
        }
    }
}
