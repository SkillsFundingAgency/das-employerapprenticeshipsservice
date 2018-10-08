namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class Constants
    {
        public static class ObjectContextKeys
        {
            public const string EmpRef = "empref";
        }

        /// <summary>
        ///     All PAYE refs used by the acceptance tests will end with this. This allows
        ///     the tests to manage the LD and txn line data without impacting non-acceptance
        ///     test data.
        /// </summary>
        public const string EmpRefPrefix = "ACP";
    }
}