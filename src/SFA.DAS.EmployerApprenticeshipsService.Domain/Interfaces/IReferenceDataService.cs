using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.ReferenceData;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IReferenceDataService
    {
        Task<Charity> GetCharity(int registrationNumber);
    }
}
