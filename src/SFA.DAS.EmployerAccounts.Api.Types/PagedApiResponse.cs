using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Api.Types
{
    public class PagedApiResponse<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}
