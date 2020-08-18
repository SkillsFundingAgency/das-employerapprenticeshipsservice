using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Services;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.DependencyResolution
{
    public class ServicesRegistry : Registry
    {
        public ServicesRegistry()
        {
            For<IContentService>().Use<ContentService>();
            For<IContentService>().DecorateAllWith<ContentServiceWithCaching>();
        }
    }
}
