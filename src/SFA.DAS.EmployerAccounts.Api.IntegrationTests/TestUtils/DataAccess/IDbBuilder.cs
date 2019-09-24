namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess
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