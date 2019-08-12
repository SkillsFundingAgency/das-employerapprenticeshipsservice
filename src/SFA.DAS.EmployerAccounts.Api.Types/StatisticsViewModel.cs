
namespace SFA.DAS.EmployerAccounts.Api.Types
{
    public class StatisticsViewModel
    {
        public long TotalAccounts { get; set; }
        public long TotalLegalEntities { get; set; }
        public long TotalPayeSchemes { get; set; }
        public long TotalAgreements { get; set; }
        // will come from finance in old api
        //public long TotalPayments { get; set; }
    }
}
