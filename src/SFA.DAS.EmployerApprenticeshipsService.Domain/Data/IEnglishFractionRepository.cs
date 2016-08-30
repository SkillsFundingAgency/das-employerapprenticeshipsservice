using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IEnglishFractionRepository
    {
        Task<DateTime> GetLastUpdateDate();
        Task<DasEnglishFraction> GetEmployerFraction(DateTime dateCalculated, string employerReference);
        Task<IEnumerable<DasEnglishFraction>> GetAllEmployerFractions(string employerReference);
        Task CreateEmployerFraction(DasEnglishFraction fractions, string employerReference);
    }
}
