using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.ReferenceData
{
    public class PagedResponse<T>
    {
        public ICollection<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
