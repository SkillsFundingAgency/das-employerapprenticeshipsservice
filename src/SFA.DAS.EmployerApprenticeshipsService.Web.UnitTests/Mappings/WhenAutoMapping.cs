using AutoMapper;
using NUnit.Framework;

namespace SFA.DAS.EAS.Web.UnitTests.Mappings
{
    [TestFixture]
    public class WhenAutoMapping
    {
        [Test]
        public void ThenShouldUseValidConfiguration()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}