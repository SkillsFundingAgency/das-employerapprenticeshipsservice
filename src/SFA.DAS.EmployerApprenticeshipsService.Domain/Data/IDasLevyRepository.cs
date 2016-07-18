﻿using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IDasLevyRepository : IRepository
    {
        Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef);
        Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef);
        Task<DasEnglishFractions> GetEmployerFraction(DateTime dateCalculated, string empRef);
        Task CreateEmployerFraction(DasEnglishFractions fractions, string empRef);
    }
}
