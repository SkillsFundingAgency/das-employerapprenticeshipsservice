using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EAS.PaymentUpdater.WebJob.DependencyResolution
{
    public static class IoC
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";

        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>(ServiceName));
                c.Policies.Add<LoggingPolicy>();
                c.Policies.Add(new MessagePolicy<EmployerApprenticeshipsServiceConfiguration>(ServiceName));
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
