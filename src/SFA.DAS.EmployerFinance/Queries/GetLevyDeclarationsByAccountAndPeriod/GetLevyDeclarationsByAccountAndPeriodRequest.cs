using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Queries.GetLevyDeclarationsByAccountAndPeriod
{
    public class GetLevyDeclarationsByAccountAndPeriodRequest : IAsyncRequest<GetLevyDeclarationsByAccountAndPeriodResponse>
    {
        public string HashedAccountId { get; set; }
        public string PayrollYear { get; set; }
        public short PayrollMonth { get; set; }
    }
}
