using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class DasLevyRepository : IDasLevyRepository
    {
        readonly string _connectionString = String.Empty;
        public DasLevyRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef)
        {
            throw new NotImplementedException();
        }

        public Task CreateEmployerDeclaration(DasDeclaration dasDeclaration)
        {
            throw new NotImplementedException();
        }
    }
}
