using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.DeleteApprentice
{
    public class DeleteApprenticeCommand : IAsyncRequest
    {
        public long AccountId { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}
