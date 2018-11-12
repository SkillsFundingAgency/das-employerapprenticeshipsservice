using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.UnitTests.Extensions
{
    [TestFixture]
    public class EndpointConfigurationExtensionTests
    {
        [TestCase("abc", "", false)]
        [TestCase("$(abc)", "ABC", true)]
        [TestCase("$(abc.123)", "ABC_123", true)]
        public void TryToInterpretConfigValueAsEnvVariable(string configValue, string expectedVariable, bool expectedResult)
        {
            // act

            var actualResult =
                EndpointConfigurationExtensions.TryToInterpretConfigValueAsEnvVariable(configValue, out var actualVariable);

            Assert.AreEqual(expectedResult, actualResult, "Determination of whether string is a config variable");

            if (actualResult && expectedResult)
            {
                Assert.AreEqual(expectedVariable, actualVariable, "Determination of config variable");
            }
        }
    }
}
