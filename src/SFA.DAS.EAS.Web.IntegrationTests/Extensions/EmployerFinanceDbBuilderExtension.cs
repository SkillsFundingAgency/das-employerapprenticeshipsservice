using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.Extensions
{
    internal static class EmployeFinanceDbBuilderExtension
    {
        internal static EmployerFinanceDbBuilder EnsureTransaction(this EmployerFinanceDbBuilder builder)
        {
            if (!builder.HasTransaction())
            {
                builder.BeginTransaction();
            }

            return builder;
        }
    }
}
