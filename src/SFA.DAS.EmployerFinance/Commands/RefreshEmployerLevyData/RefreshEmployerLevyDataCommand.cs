using System.Collections.Generic;
using MediatR;
using SFA.DAS.EmployerFinance.Models.HmrcLevy;

namespace SFA.DAS.EmployerFinance.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public ICollection<EmployerLevyData> EmployerLevyData { get; set; }
    }
}
