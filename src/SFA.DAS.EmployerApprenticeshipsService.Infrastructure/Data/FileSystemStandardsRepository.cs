﻿using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class FileSystemStandardsRepository : FileSystemRepository, IStandardsRepository
    {
        private const string fileName = "standards";

        public FileSystemStandardsRepository()
            : base("Standards")
        {
        }

        public async Task<Standard[]> GetAllAsync()
        {
            return await ReadFileById<Standard[]>(fileName);
        }

        public async Task<Standard> GetByCodeAsync(int code)
        {
            var standards = await ReadFileById<Standard[]>(fileName);

            return standards.SingleOrDefault(x => x.Code == code);
        }
    }
}