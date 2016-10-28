using System.Collections.Generic;
using MediatR;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData
{
    public class RefreshEmployerLevyDataCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public List<EmployerLevyData> EmployerLevyData { get; set; }
    }
}
