using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Mappings;

namespace SFA.DAS.EAS.Web.UnitTests.Mappings
{
    [TestFixture]
    public class WhenAutoMapping
    {
        private MapperConfiguration _config;

        [SetUp]
        public void Arrange()
        {
            _config = new MapperConfiguration(c =>
            {
                c.AddProfile<TransferConnectionMaps>();
            });
        }

        [Test]
        public void ThenAutoMappingShouldBeCorrect()
        {
            _config.AssertConfigurationIsValid();
        }
    }
}