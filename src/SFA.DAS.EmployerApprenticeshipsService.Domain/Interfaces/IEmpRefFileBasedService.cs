﻿using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IEmpRefFileBasedService
    {
        Task<string> GetEmpRef(string email, string id);
    }
}
