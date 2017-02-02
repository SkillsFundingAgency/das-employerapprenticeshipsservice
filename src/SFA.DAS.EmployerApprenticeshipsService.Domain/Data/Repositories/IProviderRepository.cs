﻿using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IProviderRepository
    {
        Task<Providers> GetAllProviders();
    }
}