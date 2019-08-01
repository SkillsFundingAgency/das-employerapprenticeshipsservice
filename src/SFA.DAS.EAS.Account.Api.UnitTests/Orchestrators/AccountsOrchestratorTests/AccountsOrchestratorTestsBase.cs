using AutoMapper;
using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.AccountsOrchestratorTests
{
    internal class AccountsOrchestratorTestsBase
    {
        protected IMapper ConfigureMapper()
        {
            var profiles = Assembly.Load("SFA.DAS.EAS.Account.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            return config.CreateMapper();
        }
    }
}
