using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerFinance.Dtos
{
    public class AccountDetailDto
    {
        public long AccountId { get; set; }
        public string HashedId { get; set; }
        public string PublicHashedId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
    }
}
