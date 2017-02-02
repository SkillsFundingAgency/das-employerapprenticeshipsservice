﻿using System.Threading.Tasks;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class FileSystemProviderRepository : FileSystemRepository, IProviderRepository
    {
        private const string DataFileName = "provider_data";

        public FileSystemProviderRepository() 
            : base("Providers")
        {
        }

        public async Task<Providers> GetAllProviders()
        {
            return await ReadFileById<Providers>(DataFileName);
        }
    }
}