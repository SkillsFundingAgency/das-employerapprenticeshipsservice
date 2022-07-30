using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Finance.Api.Types
{
    public class PagedApiResponseViewModel<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}
