using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using StructureMap;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess
{
    internal class DbBuilderRuntime
    {
        private readonly IContainer _container;

        public DbBuilderRuntime()
        {
            _container = TestSetupIoC.CreateIoC();
        }

        public async Task RunDbBuilder<TDbBuilder>(Func<TDbBuilder, Task> initialiseAction) where TDbBuilder : IDbBuilder
        {
            var builder = _container.GetInstance<TDbBuilder>();

            builder.BeginTransaction();
            try
            {
                await initialiseAction(builder);
                builder.CommitTransaction();
            }
            catch (Exception)
            {
                builder.RollbackTransaction();
                throw;
            }
        }
    }
}