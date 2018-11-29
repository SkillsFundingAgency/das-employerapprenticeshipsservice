using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.Extensions
{
    internal static class DbBuilderExtension
    {
        internal static DbBuilder EnsureTransaction(this DbBuilder builder)
        {
            if (!builder.HasTransaction())
            {
                builder.BeginTransaction();
            }

            return builder;
        }
    }
}
