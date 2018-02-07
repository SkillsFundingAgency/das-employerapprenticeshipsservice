using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Mappings;

namespace SFA.DAS.EAS.Application.UnitTests.Mappings
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
                c.AddProfile<AccountMaps>();
                c.AddProfile<TransferConnectionInvitationMaps>();
                c.AddProfile<UserMaps>();
            });
        }

        [Test]
        public void ThenAutoMappingShouldBeCorrect()
        {
            _config.AssertConfigurationIsValid();
        }
    }
}