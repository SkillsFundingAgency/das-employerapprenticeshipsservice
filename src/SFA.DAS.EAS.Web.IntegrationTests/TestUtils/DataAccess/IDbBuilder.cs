namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess
{
    /// <summary>
    ///     Represents a db builder that will be used by the test harness.
    /// </summary>
    public interface IDbBuilder
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}