﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IDasLevyRepository
    {
        Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef);
        Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef, long accountId);
       
        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId);
        Task<DasDeclaration> GetLastSubmissionForScheme(string empRef);
    }
}
