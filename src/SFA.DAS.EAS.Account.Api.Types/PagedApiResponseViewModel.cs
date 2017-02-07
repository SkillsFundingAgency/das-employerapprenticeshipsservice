using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class PagedApiResponseViewModel<T> : IAccountResource
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}
