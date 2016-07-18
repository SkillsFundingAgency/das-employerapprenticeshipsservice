using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LevyDeclarationProvider.Worker
{
    public class EmployerApprenticeshipsServiceConfiguration
    {
        public CompaniesHouseConfiguration CompaniesHouse { get; set; }
        public EmployerConfiguration Employer { get; set; }
    }

    public class EmployerConfiguration
    {
        public string DatabaseConnectionString { get; set; }
    }

    public class CompaniesHouseConfiguration
    {
        public string ApiKey { get; set; }
    }
}
