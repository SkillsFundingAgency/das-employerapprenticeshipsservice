using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IDasLevyRepository
    {
        Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef);
        Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef, long accountId);
        Task<DasEnglishFraction> GetEmployerFraction(DateTime dateCalculated, string empRef);
        Task CreateEmployerFraction(DasEnglishFraction fractions, string empRef);

        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId);
    }
}
