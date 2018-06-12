
namespace SFA.DAS.EAS.Account.Api.Types
{
    public class StatisticsViewModel
    {
        public long TotalAccounts { get; set; }
        public long TotalActiveLegalEntities { get; set; }
        public long TotalPayeSchemes { get; set; }
        public long TotalSignedAgreements { get; set; }
        public long TotalPaymentsThisYear { get; set; }
    }
}
