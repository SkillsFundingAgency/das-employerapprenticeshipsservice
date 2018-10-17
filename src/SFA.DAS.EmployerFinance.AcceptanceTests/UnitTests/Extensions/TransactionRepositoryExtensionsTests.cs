using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.UnitTests.Extensions
{
    [TestFixture]
    public class TransactionRepositoryExtensionsTests
    {
        [TestCase(  10, 20, 2, new int[] { 0, 1, 1 }, false, 0)]
        [TestCase(  60, 40, 2, new int[] { 0, 1, 1 }, false, 1)]
        [TestCase(1000,  5, 2, new int[] { 0, 1, 1}, true, 3)]
        [TestCase(1000,  5, 3, new int[] { 0, 0, 1, 1, 1 }, true, 5)]
        [TestCase(1000,  5, 3, new int[] { 0, 0, 1, 2, 3 }, false, -1)]
        [TestCase(1000,  5, 3, new int[] { 0, 0, 1, 1, 2 }, false, -1)]
        public async Task WaitForStableResult_StableResultWithinTime_ShouldReturnTrue(
            int timeout, 
            int pollInterval, 
            int stableReadCount, 
            int[] resultSequence, 
            bool expectedResult,
            int expectedNumberOfCalls)
        {
            var results = new Queue<int>(resultSequence);

            if (Debugger.IsAttached)
            {
                // Ignore the timeout if we're debugging
                timeout = -1;
            }

            var cancellationTokenSource = new CancellationTokenSource(timeout);
            var actualNumberOfCalls = 0;

            Task<int> GetNextValue()
            {
                actualNumberOfCalls++;
                return Task.FromResult(results.Count > 0 ? results.Dequeue() : 0);
            }

            var actualResult = await TransactionRepositoryExtensions.WaitForStableResult(
                        GetNextValue,
                        cancellationTokenSource.Token, 
                        pollInterval, 
                        stableReadCount);

            Assert.AreEqual(expectedResult, actualResult);

            if (expectedNumberOfCalls > -1)
            {
                Assert.AreEqual(expectedNumberOfCalls, actualNumberOfCalls);
            }
        }
    }
}
