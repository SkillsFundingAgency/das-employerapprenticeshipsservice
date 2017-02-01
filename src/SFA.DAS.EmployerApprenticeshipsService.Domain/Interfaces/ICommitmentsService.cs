using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ICommitmentsService
    {
        Task DeleteEmployerApprenticeship(long employerAccountId, long apprenticeshipId);
    }
}
