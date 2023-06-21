using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class PagedApiResponseViewModel<T>
    {
        public List<AccountWithBalanceViewModel> Data { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}
