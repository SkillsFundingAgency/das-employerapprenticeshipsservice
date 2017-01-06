using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.TotpServiceTests
{
    public class WhenICreateACode
    {
        private TotpService _totpService;

        [SetUp]
        public void Arrange()
        {
            _totpService = new TotpService();
        }

        [TestCase("12345678901234567890", "90693936", "1970-01-01 00:00:59")]
        [TestCase("12345678901234567890", "93441116", "2009-02-13 23:31:30")]
        [TestCase("12345678901234567890", "38618901", "2033-05-18 03:33:20")]
        public void ThenATotpTokenIsGeneratedFromTheServiceSecretKeyAndTime(string key, string expectedTotp, string time)
        {
            //Act
            var actual = _totpService.GetCode(key, time);

            //Assert
            Assert.AreEqual(expectedTotp, actual);
        }
    }
}
