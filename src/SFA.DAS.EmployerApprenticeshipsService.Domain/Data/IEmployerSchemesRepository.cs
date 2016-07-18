﻿using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IEmployerSchemesRepository
    {
        Task<Schemes> GetSchemesByEmployerId(int employerId);
    }
}