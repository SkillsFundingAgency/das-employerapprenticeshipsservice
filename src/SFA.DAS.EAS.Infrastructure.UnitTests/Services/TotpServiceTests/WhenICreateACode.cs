using NUnit.Framework;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.TotpServiceTests
{
    public class WhenICreateACode
    {
        private TotpService _totpService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                Hmrc = new HmrcConfiguration { OgdSecret = "GQ2TCMRSGQ2TELJRMRQTQLJUMI3DSLJYMYZWILJWGUYWMNBSMM3DSODDGY2DKMJSGI2DKMRNGFSGCOBNGRRDMOJNHBTDGZBNGY2TCZQ" }
            };

            _totpService = new TotpService(_configuration);
        }

        [TestCase("29303361", "1970-01-01 00:00:59")]
        [TestCase("84853786", "2009-02-13 23:31:30")]
        [TestCase("98909859", "2033-05-18 03:33:20")]

        public void ThenATotpTokenIsGeneratedFromTheServiceSecretKeyAndTime(string expectedTotp, string time)
        {
            //Act
            var actual = _totpService.GetCode(time);

            //Assert
            Assert.AreEqual(expectedTotp, actual);
        }
        
    }
}
